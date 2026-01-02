namespace HRManagementSystem.Application.DTOs.Requests.Designation;

/// <summary>
///Request DTO for updating an existing designation
/// </summary>


public class UpdateDesignationRequest
{
    public int DesignationId { get; set; }
    public string DesignationName { get; set; } = string.Empty;
    public string DesignationCode { get; set; } = string.Empty;
    public int Level { get; set; }
    public string? Description { get; set; }
}