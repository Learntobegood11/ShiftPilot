# ShiftPilot

ShiftPilot is a workforce scheduling and leave-management platform built with .NET 10, Blazor, ASP.NET Core Identity, Entity Framework Core, and SQLite.

The project focuses on business rules rather than simple CRUD. Its domain layer models employee roles, leave approval, shift constraints, conflict detection, and deterministic automatic schedule generation.

> **Project status:** Active development. The core domain model and scheduling engine are implemented, while the main management UI and some persistence/test coverage are still in progress.

## Why this project exists

Scheduling employees involves more than placing names on a calendar. A valid schedule must consider:

- job-role requirements;
- approved leave;
- overlapping shifts;
- weekly working-hour limits;
- minimum rest periods;
- employee scheduling availability;
- fair distribution of working hours.

ShiftPilot is designed to keep these rules inside testable C# domain services instead of placing business logic directly inside Blazor pages.

## Implemented

- User registration and login with ASP.NET Core Identity
- Organization and organization-membership domain models
- Owner, manager, and employee membership roles
- Organization-specific job roles
- Employee scheduling profiles
- Maximum weekly-hour and minimum-rest settings
- Leave-request lifecycle:
  - pending;
  - approved;
  - rejected;
  - cancelled
- Schedule, shift, and shift-assignment domain models
- Draft and published schedule states
- Scheduling conflict detection for:
  - unavailable employees;
  - job-role mismatch;
  - overlapping shifts;
  - approved leave;
  - weekly-hour limits;
  - insufficient rest
- Deterministic automatic schedule generation
- Candidate scoring and fairer assignment distribution
- Reporting of unfilled shifts and reasons
- EF Core with a local SQLite development database
- Unit-test and integration-test projects
- Repository-local `dotnet-ef` tool configuration

## In progress

- EF Core persistence for schedules, shifts, and shift assignments
- Database migrations for the latest domain entities
- Unit tests for leave requests, conflict detection, and schedule generation
- Organization-specific authorization policies
- Blazor pages for:
  - organizations;
  - employees;
  - job roles;
  - leave requests;
  - schedules;
  - automatic generation
- GitHub Actions continuous integration
- Public deployment and demo data

## Scheduling engine

The scheduling engine evaluates each employee against hard constraints before assigning a shift.

### Hard constraints

An employee is rejected when:

- scheduling is disabled for the employee;
- the employee does not have the required job role;
- the new shift overlaps an existing shift;
- the employee has approved leave;
- the assignment exceeds the weekly-hour limit;
- the assignment violates the minimum-rest requirement.

### Automatic assignment

For every unfilled shift, the generator:

1. loads eligible employees from the same organization;
2. runs conflict detection;
3. removes candidates with hard conflicts;
4. calculates a deterministic score;
5. prefers employees with fewer scheduled hours;
6. creates proposed assignments;
7. reports positions that could not be filled.

The generator is intentionally deterministic and explainable. It is not presented as artificial intelligence.

## Architecture

```text
ShiftPilot/
├── src/
│   ├── ShiftPilot.Domain/
│   │   ├── Employees/
│   │   ├── Leave/
│   │   ├── Organizations/
│   │   └── Scheduling/
│   │       └── Generation/
│   └── ShiftPilot.Web/
│       ├── Components/
│       ├── Data/
│       └── wwwroot/
├── tests/
│   ├── ShiftPilot.UnitTests/
│   └── ShiftPilot.IntegrationTests/
└── ShiftPilot.slnx
```

### Project responsibilities

- **ShiftPilot.Domain** — entities, state transitions, validation, scheduling rules, conflict detection, and generation algorithms.
- **ShiftPilot.Web** — Blazor UI, authentication, Entity Framework Core, SQLite, and application startup.
- **ShiftPilot.UnitTests** — isolated business-rule tests.
- **ShiftPilot.IntegrationTests** — web-host and database integration tests.

## Technology stack

- .NET 10
- C#
- ASP.NET Core
- Blazor Interactive Server
- ASP.NET Core Identity
- Entity Framework Core
- SQLite
- xUnit

Docker is not required for local development.

## Getting started

### Prerequisites

- .NET 10 SDK
- Git

Verify the SDK:

```powershell
dotnet --version
```

### Clone and restore

```powershell
git clone https://github.com/Learntobegood11/ShiftPilot.git
cd ShiftPilot

dotnet restore
dotnet tool restore
```

### Create or update the local database

```powershell
dotnet tool run dotnet-ef database update `
  --project src\ShiftPilot.Web `
  --startup-project src\ShiftPilot.Web
```

### Build and test

```powershell
dotnet build
dotnet test
```

### Run the application

```powershell
dotnet watch --project src\ShiftPilot.Web
```

Open the HTTPS address shown in the terminal.

## Core domain concepts

### Membership role

Controls what a user may do inside an organization:

- Owner
- Manager
- Employee

### Job role

Controls which shifts an employee may be assigned to:

- Cashier
- Cook
- Nurse
- Technician
- Receptionist

A user's membership role and job role are separate concepts.

## Roadmap

1. Add scheduling entities to `ApplicationDbContext`.
2. Create and apply the scheduling migration.
3. Add unit tests for the leave and scheduling engines.
4. Build organization and employee management pages.
5. Build leave-request submission and review pages.
6. Build schedule creation and automatic-generation pages.
7. Add organization-level authorization.
8. Add GitHub Actions.
9. Deploy a public demonstration version.

## Portfolio focus

ShiftPilot is intended to demonstrate:

- C# domain modeling;
- encapsulated business rules;
- state-based workflows;
- scheduling algorithms;
- multi-tenant data design;
- Entity Framework Core;
- authentication and authorization;
- automated testing;
- maintainable .NET project structure.
