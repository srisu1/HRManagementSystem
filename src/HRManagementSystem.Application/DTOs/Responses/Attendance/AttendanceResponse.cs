namespace HRManagementSystem.Application.DTOs.Responses.Attendance;

/// <summary>
///Response DTO for attendance information
/// </summary>

public class AttendanceResponse
{
    public int AttendanceId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? WorkHours { get; set; }
    public string? Notes { get; set; }
    
    //Additional info
    public string? EmployeeName { get; set; }
    public string? EmployeeCode { get; set; }
    public string? DepartmentName { get; set; }
    public string? DesignationName { get; set; }
}