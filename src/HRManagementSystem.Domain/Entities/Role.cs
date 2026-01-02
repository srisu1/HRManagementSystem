
using HRManagementSystem.Domain.Common;

namespace HRManagementSystem.Domain.Entities;

/// <summary>
///This class represents a user role (Admin, HR, Accountant, Staff)
/// </summary>

public class Role : BaseEntity, IAuditableEntity
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    
    //It is a navigation property links to users with this role
    public ICollection<User> Users { get; set; } = new List<User>();
}