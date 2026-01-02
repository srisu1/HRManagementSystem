using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Application.DTOs.Requests.Attendance;
using HRManagementSystem.Application.DTOs.Responses.Attendance;

namespace HRManagementSystem.Application.Interfaces.Services;

/// <summary>
/// Attendance service interface
/// Handles business logic for attendance operations
/// </summary>

public interface IAttendanceService
{
    Task<int> CheckInAsync(CheckInRequest request);
    Task<bool> CheckOutAsync(CheckOutRequest request);
    Task<AttendanceResponse?> GetTodayAttendanceAsync(int employeeId);
    Task<PagedResult<AttendanceResponse>> GetMyAttendanceHistoryAsync(int employeeId, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);
    Task<PagedResult<AttendanceResponse>> GetAttendanceByDateAsync(DateTime date, int? departmentId, int pageNumber, int pageSize);
    Task<PagedResult<AttendanceResponse>> GetTeamAttendanceAsync(int managerId, DateTime date, int pageNumber, int pageSize);
    Task<AttendanceSummaryResponse> GetAttendanceSummaryAsync(int employeeId, int month, int year);
}