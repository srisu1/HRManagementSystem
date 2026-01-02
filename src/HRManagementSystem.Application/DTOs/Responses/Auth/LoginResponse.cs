namespace HRManagementSystem.Application.DTOs.Responses.Auth;

/// <summary>
///This is the login response DTO
///It contains JWT token and user info after login is successful
/// </summary>

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}