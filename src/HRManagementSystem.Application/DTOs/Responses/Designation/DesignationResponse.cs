namespace HRManagementSystem.Application.DTOs.Responses.Designation;

/// <summary>
///Response DTO for designation information
/// </summary>
public class DesignationResponse
{
    public int DesignationId { get; set; }
    public string DesignationName { get; set; } = string.Empty;
    public string DesignationCode { get; set; } = string.Empty;
    public int Level { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}