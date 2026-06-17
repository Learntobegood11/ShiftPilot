namespace ShiftPilot.Domain.Scheduling;

public sealed record SchedulingConflict(
    SchedulingConflictType Type,
    string Message,
    Guid? RelatedEntityId = null);
