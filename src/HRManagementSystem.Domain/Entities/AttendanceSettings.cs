using HRManagementSystem.Domain.Common;
namespace HRManagementSystem.Domain.Entities;

/// <summary>
///Attendance settings for company or branch
///Defines working hours, weekends, etc.
/// </summary>

public class AttendanceSettings : BaseEntity, IAuditableEntity
{
    public int SettingId { get; set; }
    public int? CompanyId { get; set; }
    public int? BranchId { get; set; }
    public TimeSpan WorkStartTime { get; set; }
    public TimeSpan WorkEndTime { get; set; }
    public decimal StandardWorkHours { get; set; }
    public string WeekendDays { get; set; } = string.Empty; //This will be a JSON array
    public int LateArrivalThreshold { get; set; } //In minutes
    public int EarlyDepartureThreshold { get; set; } //In minutes
    public bool IsActive { get; set; }
    
    //for navigation
    public Company? Company { get; set; }
    public Branch? Branch { get; set; }
}
