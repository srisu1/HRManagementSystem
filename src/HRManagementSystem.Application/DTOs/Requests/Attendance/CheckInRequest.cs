namespace HRManagementSystem.Application.DTOs.Requests.Attendance;

/// <summary>
///Request DTO for employee check in
/// </summary>
public class CheckInRequest
{
    public int EmployeeId { get; set; }
    public DateTime CheckInTime { get; set; }
}