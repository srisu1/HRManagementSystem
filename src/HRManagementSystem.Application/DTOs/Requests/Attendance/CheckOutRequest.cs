namespace HRManagementSystem.Application.DTOs.Requests.Attendance;

/// <summary>
///Request DTO for employee check out
/// </summary>

public class CheckOutRequest
{
    public int EmployeeId { get; set; }
    public DateTime CheckOutTime { get; set; }
}