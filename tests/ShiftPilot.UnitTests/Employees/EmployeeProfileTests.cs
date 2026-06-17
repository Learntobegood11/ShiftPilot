using ShiftPilot.Domain.Employees;

namespace ShiftPilot.UnitTests.Employees;

public sealed class EmployeeProfileTests
{
    [Fact]
    public void Constructor_WithValidValues_CreatesProfile()
    {
        var organizationId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var jobRoleId = Guid.NewGuid();

        var profile = new EmployeeProfile(
            organizationId,
            memberId,
            "Anna Smith",
            jobRoleId);

        Assert.NotEqual(Guid.Empty, profile.Id);
        Assert.Equal(organizationId, profile.OrganizationId);
        Assert.Equal(memberId, profile.OrganizationMemberId);
        Assert.Equal(jobRoleId, profile.JobRoleId);
        Assert.Equal("Anna Smith", profile.DisplayName);
        Assert.Equal(40, profile.MaximumWeeklyHours);
        Assert.Equal(11, profile.MinimumRestHours);
        Assert.True(profile.IsAvailableForScheduling);
    }

    [Fact]
    public void Constructor_TrimsDisplayName()
    {
        var profile = CreateProfile("  Anna Smith  ");

        Assert.Equal("Anna Smith", profile.DisplayName);
    }

    [Fact]
    public void ChangeJobRole_ChangesRole()
    {
        var profile = CreateProfile();
        var newRoleId = Guid.NewGuid();

        profile.ChangeJobRole(newRoleId);

        Assert.Equal(newRoleId, profile.JobRoleId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(81)]
    public void UpdateSchedulingRules_WithInvalidWeeklyHours_Throws(
        int invalidHours)
    {
        var profile = CreateProfile();

        var action = () => profile.UpdateSchedulingRules(
            invalidHours,
            11);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(25)]
    public void UpdateSchedulingRules_WithInvalidRestHours_Throws(
        int invalidRestHours)
    {
        var profile = CreateProfile();

        var action = () => profile.UpdateSchedulingRules(
            40,
            invalidRestHours);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void DisableScheduling_MarksEmployeeUnavailable()
    {
        var profile = CreateProfile();

        profile.DisableScheduling();

        Assert.False(profile.IsAvailableForScheduling);
    }

    [Fact]
    public void EnableScheduling_MarksEmployeeAvailable()
    {
        var profile = CreateProfile();

        profile.DisableScheduling();
        profile.EnableScheduling();

        Assert.True(profile.IsAvailableForScheduling);
    }

    private static EmployeeProfile CreateProfile(
        string displayName = "Anna Smith")
    {
        return new EmployeeProfile(
            Guid.NewGuid(),
            Guid.NewGuid(),
            displayName,
            Guid.NewGuid());
    }
}