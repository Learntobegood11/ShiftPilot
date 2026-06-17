namespace ShiftPilot.Domain.Leave;

public sealed class LeaveRequest
{
    private const int MaximumReasonLength = 500;
    private const int MaximumReviewCommentLength = 500;

    private LeaveRequest()
    {
        // Required by Entity Framework Core.
    }

    public LeaveRequest(
        Guid organizationId,
        Guid employeeProfileId,
        DateTimeOffset startUtc,
        DateTimeOffset endUtc,
        string reason)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID is required.",
                nameof(organizationId));
        }

        if (employeeProfileId == Guid.Empty)
        {
            throw new ArgumentException(
                "Employee profile ID is required.",
                nameof(employeeProfileId));
        }

        ValidatePeriod(startUtc, endUtc);

        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        EmployeeProfileId = employeeProfileId;
        StartUtc = startUtc.ToUniversalTime();
        EndUtc = endUtc.ToUniversalTime();
        Reason = NormalizeRequiredText(
            reason,
            MaximumReasonLength,
            nameof(reason));

        Status = LeaveRequestStatus.Pending;
        SubmittedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid OrganizationId { get; private set; }

    public Guid EmployeeProfileId { get; private set; }

    public DateTimeOffset StartUtc { get; private set; }

    public DateTimeOffset EndUtc { get; private set; }

    public string Reason { get; private set; } = string.Empty;

    public LeaveRequestStatus Status { get; private set; }

    public DateTimeOffset SubmittedAtUtc { get; private set; }

    public string? ReviewedByUserId { get; private set; }

    public DateTimeOffset? ReviewedAtUtc { get; private set; }

    public string? ReviewComment { get; private set; }

    public DateTimeOffset? CancelledAtUtc { get; private set; }

    public void Approve(
        string reviewedByUserId,
        string? comment = null)
    {
        EnsurePending();

        ReviewedByUserId =
            NormalizeReviewerId(reviewedByUserId);

        ReviewComment = NormalizeOptionalText(
            comment,
            MaximumReviewCommentLength,
            nameof(comment));

        ReviewedAtUtc = DateTimeOffset.UtcNow;
        Status = LeaveRequestStatus.Approved;
    }

    public void Reject(
        string reviewedByUserId,
        string? comment = null)
    {
        EnsurePending();

        ReviewedByUserId =
            NormalizeReviewerId(reviewedByUserId);

        ReviewComment = NormalizeOptionalText(
            comment,
            MaximumReviewCommentLength,
            nameof(comment));

        ReviewedAtUtc = DateTimeOffset.UtcNow;
        Status = LeaveRequestStatus.Rejected;
    }

    public void Cancel()
    {
        EnsurePending();

        Status = LeaveRequestStatus.Cancelled;
        CancelledAtUtc = DateTimeOffset.UtcNow;
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

    private void EnsurePending()
    {
        if (Status != LeaveRequestStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending leave requests can be changed.");
        }
    }

    private static void ValidatePeriod(
        DateTimeOffset startUtc,
        DateTimeOffset endUtc)
    {
        if (endUtc <= startUtc)
        {
            throw new ArgumentException(
                "Leave end must be after leave start.",
                nameof(endUtc));
        }
    }

    private static string NormalizeReviewerId(
        string reviewedByUserId)
    {
        if (string.IsNullOrWhiteSpace(reviewedByUserId))
        {
            throw new ArgumentException(
                "Reviewer user ID is required.",
                nameof(reviewedByUserId));
        }

        return reviewedByUserId.Trim();
    }

    private static string NormalizeRequiredText(
        string value,
        int maximumLength,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "A value is required.",
                parameterName);
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > maximumLength)
        {
            throw new ArgumentException(
                $"The value cannot exceed {maximumLength} characters.",
                parameterName);
        }

        return normalizedValue;
    }

    private static string? NormalizeOptionalText(
        string? value,
        int maximumLength,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > maximumLength)
        {
            throw new ArgumentException(
                $"The value cannot exceed {maximumLength} characters.",
                parameterName);
        }

        return normalizedValue;
    }
}