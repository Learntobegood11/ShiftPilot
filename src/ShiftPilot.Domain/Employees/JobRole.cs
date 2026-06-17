namespace ShiftPilot.Domain.Employees;

public sealed class JobRole
{
    private JobRole()
    {
        // Required by Entity Framework Core.
    }

    public JobRole(Guid organizationId, string name)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID is required.",
                nameof(organizationId));
        }

        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        Rename(name);
        IsActive = true;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid OrganizationId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Job role name is required.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > 80)
        {
            throw new ArgumentException(
                "Job role name cannot exceed 80 characters.",
                nameof(name));
        }

        Name = normalizedName;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Reactivate()
    {
        IsActive = true;
    }
}