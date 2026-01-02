namespace HRManagementSystem.Application.DTOs.Responses.Department;

/// <summary>
///Response DTO for department information
/// </summary>

public class DepartmentResponse
{
    public int DepartmentId { get; set; }
    public int BranchId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string DepartmentCode { get; set; } = string.Empty;
    public int? ParentDepartmentId { get; set; }
    public int? ManagerId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    
    //more info from joins
    public string? BranchName { get; set; }
    public string? BranchCode { get; set; }
    public string? ParentDepartmentName { get; set; }
    public string? ManagerName { get; set; }
}