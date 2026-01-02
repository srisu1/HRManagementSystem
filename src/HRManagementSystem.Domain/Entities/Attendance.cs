using HRManagementSystem.Domain.Common;

namespace HRManagementSystem.Domain.Entities;

/// <summary>
///Represents an attendance record for an employee
/// </summary>
public class Attendance : BaseEntity, IAuditableEntity
{
    public int AttendanceId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? WorkHours { get; set; }
    public string? Notes { get; set; }
}