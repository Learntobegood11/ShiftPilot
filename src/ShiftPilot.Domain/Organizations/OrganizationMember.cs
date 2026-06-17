namespace ShiftPilot.Domain.Organizations;

public sealed class OrganizationMember
{
    private OrganizationMember()
    {
        // Required by Entity Framework Core.
    }

    public OrganizationMember(
        Guid organizationId,
        string userId,
        MembershipRole role)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID is required.",
                nameof(organizationId));
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException(
                "User ID is required.",
                nameof(userId));
        }

        EnsureValidRole(role);

        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        UserId = userId.Trim();
        Role = role;
        JoinedAtUtc = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    public Guid Id { get; private set; }

    public Guid OrganizationId { get; private set; }

    public string UserId { get; private set; } = string.Empty;

    public MembershipRole Role { get; private set; }

    public DateTimeOffset JoinedAtUtc { get; private set; }

    public bool IsActive { get; private set; }

    public void ChangeRole(MembershipRole role)
    {
        EnsureValidRole(role);
        Role = role;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Reactivate()
    {
        IsActive = true;
    }

    private static void EnsureValidRole(MembershipRole role)
    {
        if (!Enum.IsDefined(role))
        {
            throw new ArgumentOutOfRangeException(
                nameof(role),
                role,
                "The membership role is invalid.");
        }
    }
}