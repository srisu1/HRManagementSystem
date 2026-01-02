namespace HRManagementSystem.Application.DTOs.Requests.Department;

/// <summary>
///Request DTO for creating a new department
/// </summary>

public class CreateDepartmentRequest
{
    public int BranchId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string DepartmentCode { get; set; } = string.Empty;
    public int? ParentDepartmentId { get; set; }
    public int? ManagerId { get; set; }
    public string? Description { get; set; }
}