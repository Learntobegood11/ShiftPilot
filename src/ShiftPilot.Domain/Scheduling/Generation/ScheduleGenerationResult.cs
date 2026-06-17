namespace ShiftPilot.Domain.Scheduling.Generation;

public sealed record ScheduleGenerationResult(
    IReadOnlyCollection<GeneratedAssignment> Assignments,
    IReadOnlyCollection<UnfilledShift> UnfilledShifts)
{
    public bool IsFullyStaffed =>
        UnfilledShifts.Count == 0;
}
