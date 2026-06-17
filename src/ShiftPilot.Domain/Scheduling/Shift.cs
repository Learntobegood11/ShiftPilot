namespace ShiftPilot.Domain.Scheduling;

public sealed class Shift
{
    private Shift()
    {
        // Required by Entity Framework Core.
    }

    public Shift(
        Guid organizationId,
        Guid scheduleId,
        Guid requiredJobRoleId,
        string title,
        DateTimeOffset startUtc,
        DateTimeOffset endUtc,
        int requiredEmployeeCount)
    {
        ValidateRequiredId(
            organizationId,
            nameof(organizationId),
            "Organization ID is required.");

        ValidateRequiredId(
            scheduleId,
            nameof(scheduleId),
            "Schedule ID is required.");

        ValidateRequiredId(
            requiredJobRoleId,
            nameof(requiredJobRoleId),
            "Required job role ID is required.");

        ValidatePeriod(startUtc, endUtc);
        ValidateRequiredEmployeeCount(requiredEmployeeCount);

        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        ScheduleId = scheduleId;
        RequiredJobRoleId = requiredJobRoleId;
        Title = NormalizeTitle(title);
        StartUtc = startUtc.ToUniversalTime();
        EndUtc = endUtc.ToUniversalTime();
        RequiredEmployeeCount = requiredEmployeeCount;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid OrganizationId { get; private set; }

    public Guid ScheduleId { get; private set; }

    public Guid RequiredJobRoleId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public DateTimeOffset StartUtc { get; private set; }

    public DateTimeOffset EndUtc { get; private set; }

    public int RequiredEmployeeCount { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public decimal DurationHours =>
        decimal.Round(
            (decimal)(EndUtc - StartUtc).TotalHours,
            2);

    public void Rename(string title)
    {
        Title = NormalizeTitle(title);
    }

    public void ChangeRequiredEmployeeCount(
        int requiredEmployeeCount)
    {
        ValidateRequiredEmployeeCount(
            requiredEmployeeCount);

        RequiredEmployeeCount =
            requiredEmployeeCount;
    }

    public void Reschedule(
        DateTimeOffset startUtc,
        DateTimeOffset endUtc)
    {
        ValidatePeriod(startUtc, endUtc);

        StartUtc = startUtc.ToUniversalTime();
        EndUtc = endUtc.ToUniversalTime();
    }

    public bool Overlaps(Shift other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return StartUtc < other.EndUtc &&
               other.StartUtc < EndUtc;
    }

    public bool Overlaps(
        DateTimeOffset startUtc,
        DateTimeOffset endUtc)
    {
        ValidatePeriod(startUtc, endUtc);

        var normalizedStart =
            startUtc.ToUniversalTime();

        var normalizedEnd =
            endUtc.ToUniversalTime();

        return StartUtc < normalizedEnd &&
               normalizedStart < EndUtc;
    }

    private static void ValidatePeriod(
        DateTimeOffset startUtc,
        DateTimeOffset endUtc)
    {
        if (endUtc <= startUtc)
        {
            throw new ArgumentException(
                "Shift end must be after shift start.",
                nameof(endUtc));
        }

        if (endUtc - startUtc > TimeSpan.FromHours(24))
        {
            throw new ArgumentException(
                "A shift cannot exceed 24 hours.",
                nameof(endUtc));
        }
    }

    private static void ValidateRequiredEmployeeCount(
        int requiredEmployeeCount)
    {
        if (requiredEmployeeCount < 1 ||
            requiredEmployeeCount > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(requiredEmployeeCount),
                "Required employee count must be between 1 and 100.");
        }
    }

    private static string NormalizeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Shift title is required.",
                nameof(title));
        }

        var normalizedTitle = title.Trim();

        if (normalizedTitle.Length > 100)
        {
            throw new ArgumentException(
                "Shift title cannot exceed 100 characters.",
                nameof(title));
        }

        return normalizedTitle;
    }

    private static void ValidateRequiredId(
        Guid value,
        string parameterName,
        string message)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(
                message,
                parameterName);
        }
    }
}
