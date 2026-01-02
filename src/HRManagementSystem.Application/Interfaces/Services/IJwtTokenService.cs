using HRManagementSystem.Domain.Entities;

namespace HRManagementSystem.Application.Interfaces.Services;

/// <summary>
/// JWT Token service interface
/// Generates and validates JWT tokens
/// </summary>

public interface IJwtTokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    int? ValidateToken(string token);
}