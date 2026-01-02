using HRManagementSystem.Domain.Common;

namespace HRManagementSystem.Domain.Entities;

/// <summary>
///Represents an employee's complete profile
///Links to User for authentication
/// </summary>

public class EmployeeProfile : BaseEntity, IAuditableEntity
{
    public int EmployeeId { get; set; }
    public int UserId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int DesignationId { get; set; }
    public int? ManagerId { get; set; }
    
    //Personal info
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    
    // Work info
    public DateTime JoinDate { get; set; }
    public DateTime? ResignationDate { get; set; }
    public bool IsActive { get; set; }
    
    // Computed property
    public string FullName => $"{FirstName} {LastName}";
    
    // Navigation
    public User User { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public Designation Designation { get; set; } = null!;
    public EmployeeProfile? Manager { get; set; }
    public ICollection<EmployeeProfile> Subordinates { get; set; } = new List<EmployeeProfile>();
    public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
}
