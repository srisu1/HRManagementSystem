using HRManagementSystem.Domain.Common;

namespace HRManagementSystem.Domain.Entities;

/// <summary>
///Represents a job position in the organization
///Has level for hierarchy/seniority
/// </summary>
public class Designation : BaseEntity, IAuditableEntity
{
    public int DesignationId { get; set; }
    public string DesignationName { get; set; } = string.Empty;
    public string DesignationCode { get; set; } = string.Empty;
    public int Level { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}