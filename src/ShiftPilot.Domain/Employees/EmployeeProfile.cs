namespace ShiftPilot.Domain.Employees;

public sealed class EmployeeProfile
{
    private EmployeeProfile()
    {
        // Required by Entity Framework Core.
    }

    public EmployeeProfile(
        Guid organizationId,
        Guid organizationMemberId,
        string displayName,
        Guid jobRoleId,
        decimal maximumWeeklyHours = 40,
        int minimumRestHours = 11)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID is required.",
                nameof(organizationId));
        }

        if (organizationMemberId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization member ID is required.",
                nameof(organizationMemberId));
        }

        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        OrganizationMemberId = organizationMemberId;

        ChangeDisplayName(displayName);
        ChangeJobRole(jobRoleId);
        UpdateSchedulingRules(
            maximumWeeklyHours,
            minimumRestHours);

        IsAvailableForScheduling = true;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid OrganizationId { get; private set; }

    public Guid OrganizationMemberId { get; private set; }

    public string DisplayName { get; private set; } = string.Empty;

    public Guid JobRoleId { get; private set; }

    public decimal MaximumWeeklyHours { get; private set; }

    public int MinimumRestHours { get; private set; }

    public bool IsAvailableForScheduling { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public void ChangeDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException(
                "Employee display name is required.",
                nameof(displayName));
        }

        var normalizedName = displayName.Trim();

        if (normalizedName.Length > 100)
        {
            throw new ArgumentException(
                "Employee display name cannot exceed 100 characters.",
                nameof(displayName));
        }

        DisplayName = normalizedName;
    }

    public void ChangeJobRole(Guid jobRoleId)
    {
        if (jobRoleId == Guid.Empty)
        {
            throw new ArgumentException(
                "Job role ID is required.",
                nameof(jobRoleId));
        }

        JobRoleId = jobRoleId;
    }

    public void UpdateSchedulingRules(
        decimal maximumWeeklyHours,
        int minimumRestHours)
    {
        if (maximumWeeklyHours <= 0 ||
            maximumWeeklyHours > 80)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumWeeklyHours),
                "Maximum weekly hours must be between 1 and 80.");
        }

        if (minimumRestHours < 0 ||
            minimumRestHours > 24)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minimumRestHours),
                "Minimum rest hours must be between 0 and 24.");
        }

        MaximumWeeklyHours = maximumWeeklyHours;
        MinimumRestHours = minimumRestHours;
    }

    public void EnableScheduling()
    {
        IsAvailableForScheduling = true;
    }

    public void DisableScheduling()
    {
        IsAvailableForScheduling = false;
    }
}