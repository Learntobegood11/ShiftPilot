namespace ShiftPilot.Domain.Scheduling;

public sealed class Schedule
{
    private Schedule()
    {
        // Required by Entity Framework Core.
    }

    public Schedule(
        Guid organizationId,
        string name,
        DateOnly startDate,
        DateOnly endDate)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID is required.",
                nameof(organizationId));
        }

        if (endDate < startDate)
        {
            throw new ArgumentException(
                "Schedule end date cannot be before start date.",
                nameof(endDate));
        }

        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        Name = NormalizeName(name);
        StartDate = startDate;
        EndDate = endDate;
        Status = ScheduleStatus.Draft;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid OrganizationId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

    public ScheduleStatus Status { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset? PublishedAtUtc { get; private set; }

    public void Rename(string name)
    {
        EnsureDraft();
        Name = NormalizeName(name);
    }

    public void Publish()
    {
        EnsureDraft();

        Status = ScheduleStatus.Published;
        PublishedAtUtc = DateTimeOffset.UtcNow;
    }

    public void EnsureDraft()
    {
        if (Status != ScheduleStatus.Draft)
        {
            throw new InvalidOperationException(
                "Published schedules cannot be modified.");
        }
    }

    public bool Contains(DateTimeOffset dateTime)
    {
        var date = DateOnly.FromDateTime(
            dateTime.UtcDateTime);

        return date >= StartDate &&
               date <= EndDate;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Schedule name is required.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > 100)
        {
            throw new ArgumentException(
                "Schedule name cannot exceed 100 characters.",
                nameof(name));
        }

        return normalizedName;
    }
}
