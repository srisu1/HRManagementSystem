using HRManagementSystem.Domain.Common;
namespace HRManagementSystem.Domain.Entities;

/// <summary>
///This class represents a company in the system
///One company can have many branches
/// </summary>


public class Company : BaseEntity, IAuditableEntity
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    
    //Navigation
    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
}