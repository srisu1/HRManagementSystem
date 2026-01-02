
using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Domain.Entities;

namespace HRManagementSystem.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for User entity
/// Extends base repository and adds user-specific operations
/// </summary>


public interface IUserRepository : IBaseRepository<User>
{
 
    //Find user by email (for login)
    Task<User?> GetByEmailAsync(string email);
    
 
    //Update login attempt counter
    //Used for security (lock account after failed attempts)
    Task<bool> UpdateLoginAttemptAsync(int userId, bool isSuccess);
    
  
    //Check if email already exists
    //Used for validation during registration
    Task<bool> EmailExistsAsync(string email);
}