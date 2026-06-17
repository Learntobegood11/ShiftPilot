namespace ShiftPilot.Domain.Scheduling.Generation;

public sealed record UnfilledShift(
    Guid ShiftId,
    int MissingEmployeeCount,
    IReadOnlyCollection<string> Reasons);
