namespace HRManagementSystem.Domain.Common;

/// <summary>
///This is a base class for all entities. It provides audit fields
///All the tables will have these fields to track who created and modified records
/// </summary>

public abstract class BaseEntity
{
    public DateTime CreatedAt {get; set;}
    public int? CreatedBy {get; set;}
    public DateTime? ModifiedAt { get; set; }
    public int? ModifiedBy { get; set; }
}