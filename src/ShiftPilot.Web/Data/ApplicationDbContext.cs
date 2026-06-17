using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShiftPilot.Domain.Employees;
using ShiftPilot.Domain.Leave;
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

    public DbSet<JobRole> JobRoles =>
        Set<JobRole>();

    public DbSet<EmployeeProfile> EmployeeProfiles =>
        Set<EmployeeProfile>();

    public DbSet<LeaveRequest> LeaveRequests =>
        Set<LeaveRequest>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureOrganization(builder);
        ConfigureOrganizationMember(builder);
        ConfigureJobRole(builder);
        ConfigureEmployeeProfile(builder);
        ConfigureLeaveRequest(builder);
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

    private static void ConfigureJobRole(
        ModelBuilder builder)
    {
        var entity = builder.Entity<JobRole>();

        entity.ToTable("JobRoles");

        entity.HasKey(jobRole => jobRole.Id);

        entity.Property(jobRole => jobRole.Name)
            .HasMaxLength(80)
            .IsRequired();

        entity.Property(jobRole => jobRole.IsActive)
            .IsRequired();

        entity.Property(jobRole => jobRole.CreatedAtUtc)
            .IsRequired();

        entity.HasIndex(jobRole => new
            {
                jobRole.OrganizationId,
                jobRole.Name
            })
            .IsUnique();

        entity.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(jobRole => jobRole.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureEmployeeProfile(
        ModelBuilder builder)
    {
        var entity = builder.Entity<EmployeeProfile>();

        entity.ToTable("EmployeeProfiles");

        entity.HasKey(profile => profile.Id);

        entity.Property(profile => profile.DisplayName)
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(profile => profile.MaximumWeeklyHours)
            .HasPrecision(5, 2)
            .IsRequired();

        entity.Property(profile => profile.MinimumRestHours)
            .IsRequired();

        entity.Property(profile => profile.IsAvailableForScheduling)
            .IsRequired();

        entity.Property(profile => profile.CreatedAtUtc)
            .IsRequired();

        entity.HasIndex(profile => profile.OrganizationMemberId)
            .IsUnique();

        entity.HasIndex(profile => new
            {
                profile.OrganizationId,
                profile.JobRoleId
            });

        entity.HasOne<OrganizationMember>()
            .WithOne()
            .HasForeignKey<EmployeeProfile>(
                profile => profile.OrganizationMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne<JobRole>()
            .WithMany()
            .HasForeignKey(profile => profile.JobRoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureLeaveRequest(
        ModelBuilder builder)
    {
        var entity = builder.Entity<LeaveRequest>();

        entity.ToTable("LeaveRequests");

        entity.HasKey(request => request.Id);

        entity.Property(request => request.StartUtc)
            .IsRequired();

        entity.Property(request => request.EndUtc)
            .IsRequired();

        entity.Property(request => request.Reason)
            .HasMaxLength(500)
            .IsRequired();

        entity.Property(request => request.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entity.Property(request => request.SubmittedAtUtc)
            .IsRequired();

        entity.Property(request => request.ReviewedByUserId)
            .HasMaxLength(450);

        entity.Property(request => request.ReviewComment)
            .HasMaxLength(500);

        entity.HasIndex(request => new
            {
                request.OrganizationId,
                request.EmployeeProfileId,
                request.StartUtc,
                request.EndUtc
            });

        entity.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(request => request.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne<EmployeeProfile>()
            .WithMany()
            .HasForeignKey(request => request.EmployeeProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}