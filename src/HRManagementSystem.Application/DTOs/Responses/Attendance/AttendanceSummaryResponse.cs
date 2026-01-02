namespace HRManagementSystem.Application.DTOs.Responses.Attendance;

/// <summary>
///Response DTO for attendance summary
/// </summary>
public class AttendanceSummaryResponse
{
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int LateDays { get; set; }
    public int AbsentDays { get; set; }
    public int LeaveDays { get; set; }
    public decimal TotalWorkHours { get; set; }
    public decimal AverageWorkHours { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}