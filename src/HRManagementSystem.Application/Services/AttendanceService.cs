using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Application.DTOs.Requests.Attendance;
using HRManagementSystem.Application.DTOs.Responses.Attendance;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Domain.Entities;

namespace HRManagementSystem.Application.Services;

/// <summary>
///Attendance service implementation
///has business logic for attendance management
/// </summary>
public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendanceRepository;

    
    public AttendanceService(IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    
    
    public async Task<int> CheckInAsync(CheckInRequest request)
    {
        return await _attendanceRepository.CheckInAsync(request.EmployeeId, request.CheckInTime);
    }

    
    
    public async Task<bool> CheckOutAsync(CheckOutRequest request)
    {
        return await _attendanceRepository.CheckOutAsync(request.EmployeeId, request.CheckOutTime);
    }

    
    
    public async Task<AttendanceResponse?> GetTodayAttendanceAsync(int employeeId)
    {
        var attendance = await _attendanceRepository.GetTodayAttendanceAsync(employeeId);
        
        if (attendance == null)
            return null;
            
        return MapToResponse(attendance);
    }

    
    
    public async Task<PagedResult<AttendanceResponse>> GetMyAttendanceHistoryAsync(int employeeId, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
    {
        var pagedAttendance = await _attendanceRepository.GetAttendanceByEmployeeAsync(employeeId, startDate, endDate, pageNumber, pageSize);
        
        var attendanceResponses = pagedAttendance.Items.Select(a => MapToResponse(a)).ToList();
        
        return new PagedResult<AttendanceResponse>
        {
            Items = attendanceResponses,
            PageNumber = pagedAttendance.PageNumber,
            PageSize = pagedAttendance.PageSize,
            TotalRecords = pagedAttendance.TotalRecords
        };
    }

    
    
    public async Task<PagedResult<AttendanceResponse>> GetAttendanceByDateAsync(DateTime date, int? departmentId, int pageNumber, int pageSize)
    {
        var pagedAttendance = await _attendanceRepository.GetAttendanceByDateAsync(date, departmentId, pageNumber, pageSize);
        
        var attendanceResponses = pagedAttendance.Items.Select(a => MapToResponse(a)).ToList();
        
        return new PagedResult<AttendanceResponse>
        {
            Items = attendanceResponses,
            PageNumber = pagedAttendance.PageNumber,
            PageSize = pagedAttendance.PageSize,
            TotalRecords = pagedAttendance.TotalRecords
        };
    }

    
    
    public async Task<PagedResult<AttendanceResponse>> GetTeamAttendanceAsync(int managerId, DateTime date, int pageNumber, int pageSize)
    {
        var pagedAttendance = await _attendanceRepository.GetTeamAttendanceAsync(managerId, date, pageNumber, pageSize);
        
        var attendanceResponses = pagedAttendance.Items.Select(a => MapToResponse(a)).ToList();
        
        return new PagedResult<AttendanceResponse>
        {
            Items = attendanceResponses,
            PageNumber = pagedAttendance.PageNumber,
            PageSize = pagedAttendance.PageSize,
            TotalRecords = pagedAttendance.TotalRecords
        };
    }

    
    
    public async Task<AttendanceSummaryResponse> GetAttendanceSummaryAsync(int employeeId, int month, int year)
    {
        return await _attendanceRepository.GetAttendanceSummaryAsync(employeeId, month, year);
    }

    
    
    private AttendanceResponse MapToResponse(Attendance attendance)
    {
        return new AttendanceResponse
        {
            AttendanceId = attendance.AttendanceId,
            EmployeeId = attendance.EmployeeId,
            AttendanceDate = attendance.AttendanceDate,
            CheckInTime = attendance.CheckInTime,
            CheckOutTime = attendance.CheckOutTime,
            Status = attendance.Status,
            WorkHours = attendance.WorkHours,
            Notes = attendance.Notes,
            EmployeeName = null,
            EmployeeCode = null,
            DepartmentName = null,
            DesignationName = null
        };
    }
}