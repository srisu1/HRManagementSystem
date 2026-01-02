using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Application.DTOs.Responses.Attendance;
using HRManagementSystem.Domain.Entities;

namespace HRManagementSystem.Application.Interfaces.Repositories;

/// <summary>
///Repository interface for Attendance entity
/// </summary>
public interface IAttendanceRepository : IBaseRepository<Attendance>
{
    Task<int> CheckInAsync(int employeeId, DateTime checkInTime);
    Task<bool> CheckOutAsync(int employeeId, DateTime checkOutTime);
    Task<Attendance?> GetTodayAttendanceAsync(int employeeId);
    Task<PagedResult<Attendance>> GetAttendanceByEmployeeAsync(int employeeId, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);
    Task<PagedResult<Attendance>> GetAttendanceByDateAsync(DateTime date, int? departmentId, int pageNumber, int pageSize);
    Task<PagedResult<Attendance>> GetTeamAttendanceAsync(int managerId, DateTime date, int pageNumber, int pageSize);
    Task<AttendanceSummaryResponse> GetAttendanceSummaryAsync(int employeeId, int month, int year);
}