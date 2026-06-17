namespace ShiftPilot.Domain.Scheduling.Generation;

public sealed record GeneratedAssignment(
    Guid ShiftId,
    Guid EmployeeProfileId,
    decimal Score);
