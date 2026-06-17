using ShiftPilot.Domain.Organizations;

namespace ShiftPilot.UnitTests.Organizations;

public sealed class OrganizationTests
{
    [Fact]
    public void Constructor_WithValidInput_CreatesOrganization()
    {
        var organization = new Organization(
            "Northwind Café",
            "Europe/Berlin");

        Assert.NotEqual(Guid.Empty, organization.Id);
        Assert.Equal("Northwind Café", organization.Name);
        Assert.Equal("Europe/Berlin", organization.TimeZoneId);
    }

    [Fact]
    public void Constructor_TrimsOrganizationName()
    {
        var organization = new Organization(
            "  Northwind Café  ",
            "Europe/Berlin");

        Assert.Equal("Northwind Café", organization.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("     ")]
    public void Constructor_WithEmptyName_ThrowsArgumentException(
        string invalidName)
    {
        var action = () => new Organization(
            invalidName,
            "Europe/Berlin");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Rename_WithValidName_ChangesName()
    {
        var organization = new Organization(
            "Old Company",
            "Europe/Berlin");

        organization.Rename("New Company");

        Assert.Equal("New Company", organization.Name);
    }

    [Fact]
    public void Rename_WithNameLongerThanOneHundredCharacters_Throws()
    {
        var organization = new Organization(
            "Old Company",
            "Europe/Berlin");

        var invalidName = new string('A', 101);

        var action = () => organization.Rename(invalidName);

        Assert.Throws<ArgumentException>(action);
    }
}