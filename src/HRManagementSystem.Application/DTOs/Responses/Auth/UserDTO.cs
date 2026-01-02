namespace HRManagementSystem.Application.DTOs.Responses.Auth;

/// <summary>
///This is the user information DTO withou the sensitive data
/// </summary>

public class UserDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public int? EmployeeId { get; set; }
    public string? FullName { get; set; }
}
