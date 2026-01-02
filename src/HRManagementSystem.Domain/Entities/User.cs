using HRManagementSystem.Domain.Common;
namespace HRManagementSystem.Domain.Entities;



/// <summary>
///This class represents a user account in the system
///It contains login credentials and authentication info
/// </summary>

public class User : BaseEntity, IAuditableEntity
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public bool IsActive { get; set; }
    
    
    //security fields
    public DateTime? LastLoginAt { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }
    
    //These are the navigation properties that links to related data
    public Role Role { get; set; } = null!;
    public EmployeeProfile? EmployeeProfile { get; set; }
}