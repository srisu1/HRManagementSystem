namespace HRManagementSystem.Application.DTOs.Requests.Department;

/// <summary>
///Request DTO for updating an existing department
/// </summary>

public class UpdateDepartmentRequest
{
    public int DepartmentId { get; set; }
    public int BranchId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string DepartmentCode { get; set; } = string.Empty;
    public int? ParentDepartmentId { get; set; }
    public int? ManagerId { get; set; }
    public string? Description { get; set; }
}