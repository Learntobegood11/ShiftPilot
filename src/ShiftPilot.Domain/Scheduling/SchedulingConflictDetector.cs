using ShiftPilot.Domain.Employees;
using ShiftPilot.Domain.Leave;

namespace ShiftPilot.Domain.Scheduling;

public sealed class SchedulingConflictDetector
{
    public IReadOnlyCollection<SchedulingConflict> Detect(
        EmployeeProfile employee,
        Shift proposedShift,
        IEnumerable<Shift> assignedShifts,
        IEnumerable<LeaveRequest> leaveRequests)
    {
        ArgumentNullException.ThrowIfNull(employee);
        ArgumentNullException.ThrowIfNull(proposedShift);
        ArgumentNullException.ThrowIfNull(assignedShifts);
        ArgumentNullException.ThrowIfNull(leaveRequests);

        if (employee.OrganizationId != proposedShift.OrganizationId)
        {
            throw new ArgumentException(
                "Employee and shift must belong to the same organization.");
        }

        var conflicts = new List<SchedulingConflict>();

        var existingShifts = assignedShifts
            .Where(shift => shift.Id != proposedShift.Id)
            .OrderBy(shift => shift.StartUtc)
            .ToList();

        var employeeLeaveRequests = leaveRequests
            .Where(request =>
                request.EmployeeProfileId == employee.Id)
            .ToList();

        DetectEmployeeAvailability(
            employee,
            conflicts);

        DetectJobRoleMismatch(
            employee,
            proposedShift,
            conflicts);

        var hasOverlap = DetectOverlappingShift(
            proposedShift,
            existingShifts,
            conflicts);

        DetectApprovedLeave(
            proposedShift,
            employeeLeaveRequests,
            conflicts);

        DetectWeeklyHours(
            employee,
            proposedShift,
            existingShifts,
            conflicts);

        if (!hasOverlap)
        {
            DetectRestPeriod(
                employee,
                proposedShift,
                existingShifts,
                conflicts);
        }

        return conflicts;
    }

    private static void DetectEmployeeAvailability(
        EmployeeProfile employee,
        ICollection<SchedulingConflict> conflicts)
    {
        if (employee.IsAvailableForScheduling)
        {
            return;
        }

        conflicts.Add(
            new SchedulingConflict(
                SchedulingConflictType.EmployeeUnavailable,
                $"{employee.DisplayName} is disabled for scheduling.",
                employee.Id));
    }

    private static void DetectJobRoleMismatch(
        EmployeeProfile employee,
        Shift proposedShift,
        ICollection<SchedulingConflict> conflicts)
    {
        if (employee.JobRoleId ==
            proposedShift.RequiredJobRoleId)
        {
            return;
        }

        conflicts.Add(
            new SchedulingConflict(
                SchedulingConflictType.JobRoleMismatch,
                $"{employee.DisplayName} does not have the required job role.",
                employee.JobRoleId));
    }

    private static bool DetectOverlappingShift(
        Shift proposedShift,
        IEnumerable<Shift> existingShifts,
        ICollection<SchedulingConflict> conflicts)
    {
        var overlappingShift = existingShifts
            .FirstOrDefault(proposedShift.Overlaps);

        if (overlappingShift is null)
        {
            return false;
        }

        conflicts.Add(
            new SchedulingConflict(
                SchedulingConflictType.OverlappingShift,
                $"The proposed shift overlaps '{overlappingShift.Title}'.",
                overlappingShift.Id));

        return true;
    }

    private static void DetectApprovedLeave(
        Shift proposedShift,
        IEnumerable<LeaveRequest> leaveRequests,
        ICollection<SchedulingConflict> conflicts)
    {
        var approvedLeave = leaveRequests.FirstOrDefault(
            request =>
                request.Status == LeaveRequestStatus.Approved &&
                request.Overlaps(
                    proposedShift.StartUtc,
                    proposedShift.EndUtc));

        if (approvedLeave is null)
        {
            return;
        }

        conflicts.Add(
            new SchedulingConflict(
                SchedulingConflictType.ApprovedLeave,
                "The employee has approved leave during this shift.",
                approvedLeave.Id));
    }

    private static void DetectWeeklyHours(
        EmployeeProfile employee,
        Shift proposedShift,
        IEnumerable<Shift> existingShifts,
        ICollection<SchedulingConflict> conflicts)
    {
        var weekStart =
            GetWeekStartUtc(proposedShift.StartUtc);

        var weekEnd = weekStart.AddDays(7);

        var existingHours = existingShifts.Sum(
            shift => CalculateHoursInsidePeriod(
                shift,
                weekStart,
                weekEnd));

        var proposedHours = CalculateHoursInsidePeriod(
            proposedShift,
            weekStart,
            weekEnd);

        var totalHours = existingHours + proposedHours;

        if (totalHours <= employee.MaximumWeeklyHours)
        {
            return;
        }

        conflicts.Add(
            new SchedulingConflict(
                SchedulingConflictType.WeeklyHoursExceeded,
                $"Assignment would result in {totalHours:0.##} weekly hours. " +
                $"The employee limit is {employee.MaximumWeeklyHours:0.##} hours.",
                employee.Id));
    }

    private static void DetectRestPeriod(
        EmployeeProfile employee,
        Shift proposedShift,
        IReadOnlyCollection<Shift> existingShifts,
        ICollection<SchedulingConflict> conflicts)
    {
        if (employee.MinimumRestHours == 0)
        {
            return;
        }

        var requiredRest =
            TimeSpan.FromHours(employee.MinimumRestHours);

        var previousShift = existingShifts
            .Where(shift =>
                shift.EndUtc <= proposedShift.StartUtc)
            .OrderByDescending(shift => shift.EndUtc)
            .FirstOrDefault();

        if (previousShift is not null)
        {
            var restBefore =
                proposedShift.StartUtc - previousShift.EndUtc;

            if (restBefore < requiredRest)
            {
                conflicts.Add(
                    new SchedulingConflict(
                        SchedulingConflictType.InsufficientRestPeriod,
                        $"Only {restBefore.TotalHours:0.##} hours of rest exist " +
                        $"before the shift. {employee.MinimumRestHours} hours are required.",
                        previousShift.Id));
            }
        }

        var nextShift = existingShifts
            .Where(shift =>
                shift.StartUtc >= proposedShift.EndUtc)
            .OrderBy(shift => shift.StartUtc)
            .FirstOrDefault();

        if (nextShift is null)
        {
            return;
        }

        var restAfter =
            nextShift.StartUtc - proposedShift.EndUtc;

        if (restAfter < requiredRest)
        {
            conflicts.Add(
                new SchedulingConflict(
                    SchedulingConflictType.InsufficientRestPeriod,
                    $"Only {restAfter.TotalHours:0.##} hours of rest exist " +
                    $"after the shift. {employee.MinimumRestHours} hours are required.",
                    nextShift.Id));
        }
    }

    private static DateTimeOffset GetWeekStartUtc(
        DateTimeOffset value)
    {
        var utcDate = value.UtcDateTime.Date;

        var daysSinceMonday =
            ((int)utcDate.DayOfWeek + 6) % 7;

        return new DateTimeOffset(
            utcDate.AddDays(-daysSinceMonday),
            TimeSpan.Zero);
    }

    private static decimal CalculateHoursInsidePeriod(
        Shift shift,
        DateTimeOffset periodStart,
        DateTimeOffset periodEnd)
    {
        var effectiveStart =
            shift.StartUtc > periodStart
                ? shift.StartUtc
                : periodStart;

        var effectiveEnd =
            shift.EndUtc < periodEnd
                ? shift.EndUtc
                : periodEnd;

        if (effectiveEnd <= effectiveStart)
        {
            return 0;
        }

        return decimal.Round(
            (decimal)(effectiveEnd - effectiveStart).TotalHours,
            2);
    }
}
