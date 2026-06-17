using ShiftPilot.Domain.Employees;

namespace ShiftPilot.UnitTests.Employees;

public sealed class JobRoleTests
{
    [Fact]
    public void Constructor_WithValidValues_CreatesJobRole()
    {
        var organizationId = Guid.NewGuid();

        var jobRole = new JobRole(
            organizationId,
            "Cashier");

        Assert.NotEqual(Guid.Empty, jobRole.Id);
        Assert.Equal(organizationId, jobRole.OrganizationId);
        Assert.Equal("Cashier", jobRole.Name);
        Assert.True(jobRole.IsActive);
    }

    [Fact]
    public void Constructor_TrimsName()
    {
        var jobRole = new JobRole(
            Guid.NewGuid(),
            "  Cashier  ");

        Assert.Equal("Cashier", jobRole.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidName_Throws(
        string invalidName)
    {
        var action = () => new JobRole(
            Guid.NewGuid(),
            invalidName);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Deactivate_MarksRoleInactive()
    {
        var jobRole = new JobRole(
            Guid.NewGuid(),
            "Cashier");

        jobRole.Deactivate();

        Assert.False(jobRole.IsActive);
    }

    [Fact]
    public void Reactivate_MarksRoleActive()
    {
        var jobRole = new JobRole(
            Guid.NewGuid(),
            "Cashier");

        jobRole.Deactivate();
        jobRole.Reactivate();

        Assert.True(jobRole.IsActive);
    }
}