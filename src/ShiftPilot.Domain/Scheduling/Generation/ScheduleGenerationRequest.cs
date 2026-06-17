using ShiftPilot.Domain.Employees;
using ShiftPilot.Domain.Leave;

namespace ShiftPilot.Domain.Scheduling.Generation;

public sealed record ScheduleGenerationRequest(
    Guid OrganizationId,
    IReadOnlyCollection<Shift> ShiftsToFill,
    IReadOnlyCollection<EmployeeProfile> Employees,
    IReadOnlyCollection<Shift> AllShifts,
    IReadOnlyCollection<ShiftAssignment> ExistingAssignments,
    IReadOnlyCollection<LeaveRequest> LeaveRequests);