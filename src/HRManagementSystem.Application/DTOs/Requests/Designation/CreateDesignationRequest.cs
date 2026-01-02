namespace HRManagementSystem.Application.DTOs.Requests.Designation;

/// <summary>
///Request DTO for creating a new designation
/// </summary>

public class CreateDesignationRequest
{
    public string DesignationName { get; set; } = string.Empty;
    public string DesignationCode { get; set; } = string.Empty;
    public int Level { get; set; }
    public string? Description { get; set; }
}