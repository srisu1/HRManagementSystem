using HRManagementSystem.Application.DTOs.Requests.Auth;
using HRManagementSystem.Application.DTOs.Responses.Auth;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;

namespace HRManagementSystem.Application.Services;

/// <summary>
///Authentication service implementation
///Handles user login and authentication logic
/// </summary>


public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    
    
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        //Get user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);

        //check user exists
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        //Check if account is locked
        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            var remainingMinutes = (user.LockoutEnd.Value - DateTime.UtcNow).Minutes;
            throw new UnauthorizedAccessException($"Account is locked. Try again in {remainingMinutes} minutes.");
        }

        //Check if account is active
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is disabled");
        }

        //Verify password using BCrypt
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            //Update failed login attempt
            await _userRepository.UpdateLoginAttemptAsync(user.UserId, false);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        //Update successful login
        await _userRepository.UpdateLoginAttemptAsync(user.UserId, true);

        //Generate JWT token
        var token = _jwtTokenService.GenerateToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        //Get employee details if exists
        var userWithDetails = await _userRepository.GetByIdAsync(user.UserId);

        return new LoginResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(8),
            User = new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                RoleName = user.Role?.RoleName ?? "Unknown",
                EmployeeId = userWithDetails?.EmployeeProfile?.EmployeeId,
                FullName = userWithDetails?.EmployeeProfile != null 
                    ? $"{userWithDetails.EmployeeProfile.FirstName} {userWithDetails.EmployeeProfile.LastName}"
                    : null
            }
        };
    }

    
    
    public async Task<UserDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            return null;

        return new UserDto
        {
            UserId = user.UserId,
            Email = user.Email,
            RoleName = user.Role?.RoleName ?? "Unknown",
            EmployeeId = user.EmployeeProfile?.EmployeeId,
            FullName = user.EmployeeProfile != null 
                ? $"{user.EmployeeProfile.FirstName} {user.EmployeeProfile.LastName}"
                : null
        };
    }

    
    
    public async Task<bool> ValidateTokenAsync(string token)
    {
        var userId = _jwtTokenService.ValidateToken(token);
        return userId.HasValue;
    }
}