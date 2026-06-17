using ShiftPilot.Domain.Organizations;

namespace ShiftPilot.UnitTests.Organizations;

public sealed class OrganizationMemberTests
{
    [Fact]
    public void Constructor_WithValidValues_CreatesActiveMember()
    {
        var organizationId = Guid.NewGuid();

        var member = new OrganizationMember(
            organizationId,
            "user-123",
            MembershipRole.Employee);

        Assert.NotEqual(Guid.Empty, member.Id);
        Assert.Equal(organizationId, member.OrganizationId);
        Assert.Equal("user-123", member.UserId);
        Assert.Equal(MembershipRole.Employee, member.Role);
        Assert.True(member.IsActive);
    }

    [Fact]
    public void Constructor_WithEmptyOrganizationId_Throws()
    {
        var action = () => new OrganizationMember(
            Guid.Empty,
            "user-123",
            MembershipRole.Employee);

        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidUserId_Throws(string userId)
    {
        var action = () => new OrganizationMember(
            Guid.NewGuid(),
            userId,
            MembershipRole.Employee);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void ChangeRole_ChangesMemberRole()
    {
        var member = CreateMember();

        member.ChangeRole(MembershipRole.Manager);

        Assert.Equal(MembershipRole.Manager, member.Role);
    }

    [Fact]
    public void Deactivate_MakesMemberInactive()
    {
        var member = CreateMember();

        member.Deactivate();

        Assert.False(member.IsActive);
    }

    [Fact]
    public void Reactivate_MakesMemberActive()
    {
        var member = CreateMember();
        member.Deactivate();

        member.Reactivate();

        Assert.True(member.IsActive);
    }

    private static OrganizationMember CreateMember()
    {
        return new OrganizationMember(
            Guid.NewGuid(),
            "user-123",
            MembershipRole.Employee);
    }
}