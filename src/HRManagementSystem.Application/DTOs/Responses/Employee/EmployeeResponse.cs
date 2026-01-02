namespace HRManagementSystem.Application.DTOs.Responses.Employee;

/// <summary>
/// Response DTO for employee profile information
/// </summary>
public class EmployeeResponse
{
    public int EmployeeId { get; set; }
    public int UserId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public int DepartmentId { get; set; }
    public int DesignationId { get; set; }
    public int? ManagerId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime JoinDate { get; set; }
    public DateTime? ResignationDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    // Additional info from joins
    public string? Email { get; set; }
    public string? DepartmentName { get; set; }
    public string? DepartmentCode { get; set; }
    public string? DesignationName { get; set; }
    public int? DesignationLevel { get; set; }
    public string? ManagerName { get; set; }
}