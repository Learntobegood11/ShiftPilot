namespace ShiftPilot.Domain.Scheduling;

public enum SchedulingConflictType
{
    EmployeeUnavailable = 1,
    JobRoleMismatch = 2,
    OverlappingShift = 3,
    ApprovedLeave = 4,
    WeeklyHoursExceeded = 5,
    InsufficientRestPeriod = 6
}
