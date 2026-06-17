namespace ShiftPilot.Domain.Organizations;

public sealed class Organization
{
    private Organization()
    {
        // Required by Entity Framework Core.
    }

    public Organization(string name, string timeZoneId)
    {
        Id = Guid.NewGuid();
        Rename(name);
        ChangeTimeZone(timeZoneId);
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string TimeZoneId { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Organization name is required.",
                nameof(name));
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > 100)
        {
            throw new ArgumentException(
                "Organization name cannot exceed 100 characters.",
                nameof(name));
        }

        Name = normalizedName;
    }

    public void ChangeTimeZone(string timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            throw new ArgumentException(
                "Time zone is required.",
                nameof(timeZoneId));
        }

        TimeZoneId = timeZoneId.Trim();
    }
}