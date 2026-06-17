namespace ShiftPilot.Domain.Scheduling;

public sealed class ShiftAssignment
{
    private ShiftAssignment()
    {
        // Required by Entity Framework Core.
    }

    public ShiftAssignment(
        Guid organizationId,
        Guid shiftId,
        Guid employeeProfileId,
        AssignmentSource source)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID is required.",
                nameof(organizationId));
        }

        if (shiftId == Guid.Empty)
        {
            throw new ArgumentException(
                "Shift ID is required.",
                nameof(shiftId));
        }

        if (employeeProfileId == Guid.Empty)
        {
            throw new ArgumentException(
                "Employee profile ID is required.",
                nameof(employeeProfileId));
        }

        if (!Enum.IsDefined(source))
        {
            throw new ArgumentOutOfRangeException(
                nameof(source),
                source,
                "Assignment source is invalid.");
        }

        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        ShiftId = shiftId;
        EmployeeProfileId = employeeProfileId;
        Source = source;
        AssignedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid OrganizationId { get; private set; }

    public Guid ShiftId { get; private set; }

    public Guid EmployeeProfileId { get; private set; }

    public AssignmentSource Source { get; private set; }

    public DateTimeOffset AssignedAtUtc { get; private set; }
}
