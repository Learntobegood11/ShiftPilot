# ShiftPilot

ShiftPilot is a workforce scheduling and leave-management
application for small and medium-sized organizations.

## Current functionality

- User registration and login
- ASP.NET Core Identity
- Organization domain model
- Organization membership model
- SQLite development database
- Entity Framework Core migrations
- Unit tests with xUnit

## Planned functionality

- Organization creation
- Employee invitations
- Owner, manager, and employee permissions
- Availability management
- Shift scheduling
- Conflict detection
- Leave requests
- Shift swaps
- Notifications
- Audit logs
- Reporting

## Technology

- .NET 10
- ASP.NET Core
- Blazor
- Entity Framework Core
- SQLite
- xUnit

## Local development

```bash
dotnet restore
dotnet ef database update \
  --project src/ShiftPilot.Web \
  --startup-project src/ShiftPilot.Web
dotnet watch --project src/ShiftPilot.Web