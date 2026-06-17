using ShiftPilot.Domain.Employees;

namespace ShiftPilot.Domain.Scheduling.Generation;

public sealed class AutomaticScheduleGenerator
{
    private readonly SchedulingConflictDetector _conflictDetector;

    public AutomaticScheduleGenerator(
        SchedulingConflictDetector conflictDetector)
    {
        _conflictDetector = conflictDetector;
    }

    public ScheduleGenerationResult Generate(
        ScheduleGenerationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.OrganizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID is required.",
                nameof(request));
        }

        var shiftsById = request.AllShifts
            .Concat(request.ShiftsToFill)
            .Where(shift =>
                shift.OrganizationId == request.OrganizationId)
            .GroupBy(shift => shift.Id)
            .ToDictionary(
                group => group.Key,
                group => group.First());

        var assignmentState = request.ExistingAssignments
            .Where(assignment =>
                assignment.OrganizationId == request.OrganizationId)
            .Select(assignment =>
                new AssignmentState(
                    assignment.ShiftId,
                    assignment.EmployeeProfileId))
            .ToList();

        var generatedAssignments =
            new List<GeneratedAssignment>();

        var unfilledShifts =
            new List<UnfilledShift>();

        foreach (var shift in request.ShiftsToFill
                     .Where(shift =>
                         shift.OrganizationId == request.OrganizationId)
                     .OrderBy(shift => shift.StartUtc)
                     .ThenBy(shift => shift.Id))
        {
            var alreadyAssignedCount = assignmentState.Count(
                assignment => assignment.ShiftId == shift.Id);

            var missingEmployeeCount =
                shift.RequiredEmployeeCount -
                alreadyAssignedCount;

            while (missingEmployeeCount > 0)
            {
                var evaluations = EvaluateCandidates(
                    request,
                    shift,
                    shiftsById,
                    assignmentState);

                var bestCandidate = evaluations
                    .Where(evaluation =>
                        evaluation.Conflicts.Count == 0)
                    .OrderByDescending(evaluation =>
                        evaluation.Score)
                    .ThenBy(evaluation =>
                        evaluation.Employee.DisplayName)
                    .ThenBy(evaluation =>
                        evaluation.Employee.Id)
                    .FirstOrDefault();

                if (bestCandidate is null)
                {
                    var reasons = evaluations
                        .SelectMany(evaluation =>
                            evaluation.Conflicts)
                        .Select(conflict =>
                            conflict.Message)
                        .Distinct()
                        .Take(10)
                        .ToList();

                    if (reasons.Count == 0)
                    {
                        reasons.Add(
                            "No eligible employees were found.");
                    }

                    unfilledShifts.Add(
                        new UnfilledShift(
                            shift.Id,
                            missingEmployeeCount,
                            reasons));

                    break;
                }

                assignmentState.Add(
                    new AssignmentState(
                        shift.Id,
                        bestCandidate.Employee.Id));

                generatedAssignments.Add(
                    new GeneratedAssignment(
                        shift.Id,
                        bestCandidate.Employee.Id,
                        bestCandidate.Score));

                missingEmployeeCount--;
            }
        }

        return new ScheduleGenerationResult(
            generatedAssignments,
            unfilledShifts);
    }

    private IReadOnlyCollection<CandidateEvaluation>
        EvaluateCandidates(
            ScheduleGenerationRequest request,
            Shift shift,
            IReadOnlyDictionary<Guid, Shift> shiftsById,
            IReadOnlyCollection<AssignmentState> assignmentState)
    {
        var evaluations =
            new List<CandidateEvaluation>();

        foreach (var employee in request.Employees.Where(
                     employee =>
                         employee.OrganizationId ==
                         request.OrganizationId))
        {
            var alreadyAssignedToShift =
                assignmentState.Any(
                    assignment =>
                        assignment.ShiftId == shift.Id &&
                        assignment.EmployeeProfileId ==
                        employee.Id);

            if (alreadyAssignedToShift)
            {
                continue;
            }

            var employeeShifts = assignmentState
                .Where(assignment =>
                    assignment.EmployeeProfileId ==
                    employee.Id)
                .Select(assignment =>
                    shiftsById.TryGetValue(
                        assignment.ShiftId,
                        out var assignedShift)
                            ? assignedShift
                            : null)
                .Where(assignedShift =>
                    assignedShift is not null)
                .Cast<Shift>()
                .ToList();

            var conflicts = _conflictDetector.Detect(
                employee,
                shift,
                employeeShifts,
                request.LeaveRequests);

            var score = conflicts.Count == 0
                ? CalculateScore(
                    employee,
                    shift,
                    employeeShifts)
                : decimal.MinValue;

            evaluations.Add(
                new CandidateEvaluation(
                    employee,
                    score,
                    conflicts));
        }

        return evaluations;
    }

    private static decimal CalculateScore(
        EmployeeProfile employee,
        Shift proposedShift,
        IReadOnlyCollection<Shift> assignedShifts)
    {
        var weekStart =
            GetWeekStartUtc(proposedShift.StartUtc);

        var weekEnd = weekStart.AddDays(7);

        var weeklyHours = assignedShifts
            .Where(shift =>
                shift.StartUtc < weekEnd &&
                shift.EndUtc > weekStart)
            .Sum(shift => shift.DurationHours);

        decimal score = 100;

        score -= weeklyHours * 2;

        var alreadyWorksThatDay =
            assignedShifts.Any(shift =>
                shift.StartUtc.UtcDateTime.Date ==
                proposedShift.StartUtc.UtcDateTime.Date);

        if (!alreadyWorksThatDay)
        {
            score += 10;
        }

        var latestPreviousShift = assignedShifts
            .Where(shift =>
                shift.EndUtc <= proposedShift.StartUtc)
            .OrderByDescending(shift =>
                shift.EndUtc)
            .FirstOrDefault();

        if (latestPreviousShift is not null)
        {
            var rest =
                proposedShift.StartUtc -
                latestPreviousShift.EndUtc;

            if (rest < TimeSpan.FromHours(16))
            {
                score -= 10;
            }
        }

        var remainingWeeklyCapacity =
            employee.MaximumWeeklyHours -
            weeklyHours;

        score += remainingWeeklyCapacity / 10;

        return score;
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

    private sealed record AssignmentState(
        Guid ShiftId,
        Guid EmployeeProfileId);

    private sealed record CandidateEvaluation(
        EmployeeProfile Employee,
        decimal Score,
        IReadOnlyCollection<SchedulingConflict> Conflicts);
}
