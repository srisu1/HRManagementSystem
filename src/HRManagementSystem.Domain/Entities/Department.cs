using HRManagementSystem.Domain.Common;

namespace HRManagementSystem.Domain.Entities;

/// <summary>
///Represents a department within a branch
///Supports hierarchical structure with parent departments
/// </summary>

public class Department : BaseEntity, IAuditableEntity
{
    public int DepartmentId { get; set; }
    public int BranchId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string DepartmentCode { get; set; } = string.Empty;
    public int? ParentDepartmentId { get; set; }
    public int? ManagerId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    //Navigation properties
    public Branch Branch { get; set; } = null!;
    public Department? ParentDepartment { get; set; }
    public ICollection<Department> ChildDepartments { get; set; } = new List<Department>();
}