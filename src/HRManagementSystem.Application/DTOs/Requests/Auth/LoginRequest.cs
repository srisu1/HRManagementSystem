namespace HRManagementSystem.Application.DTOs.Requests.Auth;

/// <summary>
///This the login request DTO
///User sends email and password to login
/// </summary>


public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
