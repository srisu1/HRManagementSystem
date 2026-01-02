using HRManagementSystem.Domain.Common;

namespace HRManagementSystem.Domain.Entities;

/// <summary>
///Represents a branch of a company
/// </summary>

public class Branch : BaseEntity, IAuditableEntity
{
    public int BranchId { get; set; }
    public int CompanyId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public ICollection<Department> Departments { get; set; } = new List<Department>();
}