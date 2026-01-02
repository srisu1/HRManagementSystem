using HRManagementSystem.Application.DTOs.Requests.Auth;
using HRManagementSystem.Application.DTOs.Responses.Auth;

namespace HRManagementSystem.Application.Interfaces.Services;

/// <summary>
/// Authentication service interface
/// Handles login, token generation, and user authentication
/// </summary>
public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<UserDto?> GetCurrentUserAsync(int userId);
    Task<bool> ValidateTokenAsync(string token);
}