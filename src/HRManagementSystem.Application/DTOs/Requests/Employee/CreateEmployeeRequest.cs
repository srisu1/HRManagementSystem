namespace HRManagementSystem.Application.DTOs.Requests.Employee;

/// <summary>
/// Request DTO for creating a new employee profile
/// </summary>


public class CreateEmployeeRequest
{
    public int UserId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int DesignationId { get; set; }
    public int? ManagerId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime JoinDate { get; set; }
}