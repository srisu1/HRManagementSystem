namespace HRManagementSystem.Domain.Common;

/// <summary>
///Since all of the entities need CreatedAt/CreatedBy and ModifiedAt/ModifiedBy
///It is an Interface for entities that need audit tracking
/// </summary>

public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    int? CreatedBy { get; set; }
    DateTime? ModifiedAt { get; set; }
    int? ModifiedBy { get; set; }
}
