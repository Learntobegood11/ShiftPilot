using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShiftPilot.Domain.Organizations;

namespace ShiftPilot.Web.Data;

public sealed class ApplicationDbContext
    : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Organization> Organizations =>
        Set<Organization>();

    public DbSet<OrganizationMember> OrganizationMembers =>
        Set<OrganizationMember>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureOrganization(builder);
        ConfigureOrganizationMember(builder);
    }

    private static void ConfigureOrganization(
        ModelBuilder builder)
    {
        var entity = builder.Entity<Organization>();

        entity.ToTable("Organizations");

        entity.HasKey(organization => organization.Id);

        entity.Property(organization => organization.Name)
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(organization => organization.TimeZoneId)
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(organization => organization.CreatedAtUtc)
            .IsRequired();
    }

    private static void ConfigureOrganizationMember(
        ModelBuilder builder)
    {
        var entity = builder.Entity<OrganizationMember>();

        entity.ToTable("OrganizationMembers");

        entity.HasKey(member => member.Id);

        entity.Property(member => member.UserId)
            .HasMaxLength(450)
            .IsRequired();

        entity.Property(member => member.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entity.Property(member => member.JoinedAtUtc)
            .IsRequired();

        entity.Property(member => member.IsActive)
            .IsRequired();

        entity.HasIndex(member => new
            {
                member.OrganizationId,
                member.UserId
            })
            .IsUnique();

        entity.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(member => member.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(member => member.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}