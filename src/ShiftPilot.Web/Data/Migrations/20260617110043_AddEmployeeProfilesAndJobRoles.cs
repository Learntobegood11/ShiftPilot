using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftPilot.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeProfilesAndJobRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobRoles_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationMemberId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    JobRoleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MaximumWeeklyHours = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    MinimumRestHours = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAvailableForScheduling = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeProfiles_JobRoles_JobRoleId",
                        column: x => x.JobRoleId,
                        principalTable: "JobRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeProfiles_OrganizationMembers_OrganizationMemberId",
                        column: x => x.OrganizationMemberId,
                        principalTable: "OrganizationMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfiles_JobRoleId",
                table: "EmployeeProfiles",
                column: "JobRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfiles_OrganizationId_JobRoleId",
                table: "EmployeeProfiles",
                columns: new[] { "OrganizationId", "JobRoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfiles_OrganizationMemberId",
                table: "EmployeeProfiles",
                column: "OrganizationMemberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobRoles_OrganizationId_Name",
                table: "JobRoles",
                columns: new[] { "OrganizationId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeProfiles");

            migrationBuilder.DropTable(
                name: "JobRoles");
        }
    }
}
