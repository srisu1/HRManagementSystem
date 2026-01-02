using Dapper;

using HRManagementSystem.Application.DTOs.Common;

using HRManagementSystem.Application.Interfaces.Repositories;

using HRManagementSystem.Domain.Entities;

using HRManagementSystem.Infrastructure.Data;

using System.Data;

namespace HRManagementSystem.Infrastructure.Repositories;

/// <summary>
/// User Repository using Dapper
/// Implements all user database operations
/// </summary>

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(DapperContext context) : base(context) { }
    
 
    //Get user by email. It is used for login and calls stored procedure sp_GetUserByEmail
   
    //Get user by email. It is used for login and calls stored procedure sp_GetUserByEmail
    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _context.CreateConnection();
    
        //Create parameters for stored procedure
        var parameters = new DynamicParameters();
        parameters.Add("Email", email);
    
        //Call stored procedure using Dapper with multi-mapping to include Role
        var result = await connection.QueryAsync<User, Role, User>(
            "sp_GetUserByEmail",
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            parameters,
            commandType: CommandType.StoredProcedure,
            splitOn: "RoleId"  // Tell Dapper where Role object starts in the result set
        );
    
        return result.FirstOrDefault();
    }
    
  
    
    //Update Login attemp counter and calls sp_UpdateLoginAttempt
    public async Task<bool> UpdateLoginAttemptAsync(int userId, bool isSuccess)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId);
        parameters.Add("IsSuccess", isSuccess);
        
        await connection.ExecuteAsync(
            "sp_UpdateLoginAttempt",
            parameters,
            commandType: CommandType.StoredProcedure
        );
        
        return true;
    }
    
 
    //Check if email already exists uses raw SQL as no stored proc needed for this  simple check
    public async Task<bool> EmailExistsAsync(string email)
    {
        using var connection = _context.CreateConnection();
        
        //Raw SQL query using Dapper
        var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Users WHERE Email = @Email) THEN 1 ELSE 0 END";
        
        var exists = await connection.ExecuteScalarAsync<bool>(sql, new { Email = email });
        
        return exists;
    }
    
 
    //Get user by ID with related data (Role, EmployeeProfile) and uses SQL with multiple joins
    public override async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        
        //SQL query with joins, Dapper will map to objects
        var sql = @"
            SELECT 
                u.*,
                r.*,
                ep.*
            FROM Users u
            INNER JOIN Roles r ON u.RoleId = r.RoleId
            LEFT JOIN EmployeeProfiles ep ON u.UserId = ep.UserId
            WHERE u.UserId = @UserId";
        
        //Dapper Multi-Mapping will map multiple objects from one query
        var user = await connection.QueryAsync<User, Role, EmployeeProfile, User>(
            sql,
            (user, role, employeeProfile) =>
            {
                user.Role = role;
                user.EmployeeProfile = employeeProfile;
                return user;
            },
            new { UserId = id },
            splitOn: "RoleId,EmployeeId" // Tell Dapper where each object starts
        );
        
        return user.FirstOrDefault();
    }
    

    //Get all users with pagination and uses raw SQL with OFFSET/FETCH
    public override async Task<PagedResult<User>> GetAllAsync(int pageNumber, int pageSize)
    {
        using var connection = _context.CreateConnection();
    
        var offset = (pageNumber - 1) * pageSize;
    
        // Multi-statement query: count + data
        var sql = @"
        SELECT COUNT(*) FROM Users;
        
        SELECT u.*, r.*
        FROM Users u
        INNER JOIN Roles r ON u.RoleId = r.RoleId
        ORDER BY u.Email
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY";
    
        //QueryMultipleAsync lets us run multiple queries at once
        using var multi = await connection.QueryMultipleAsync(
            sql, 
            new { Offset = offset, PageSize = pageSize }
        );
    
        //First result set: total count
        var totalRecords = await multi.ReadSingleAsync<int>();
    
        //second result set: user data with roles will use synchronous Read and not ReadAsync
        var users = multi.Read<User, Role, User>(
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            splitOn: "RoleId"
        ).ToList();
    
        return new PagedResult<User>
        {
            Items = users,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }
    

    //Create new user by using a  raw SQL INSERT
    public override async Task<int> CreateAsync(User entity)
    {
        using var connection = _context.CreateConnection();
        
        var sql = @"
            INSERT INTO Users (Email, PasswordHash, RoleId, IsActive, CreatedAt, CreatedBy)
            VALUES (@Email, @PasswordHash, @RoleId, @IsActive, @CreatedAt, @CreatedBy);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        
        // Dapper automatically maps object properties to SQL parameters
        var userId = await connection.ExecuteScalarAsync<int>(sql, entity);
        
        return userId;
    }
    

    //Update user using a raw sql update
    public override async Task<bool> UpdateAsync(User entity)
    {
        using var connection = _context.CreateConnection();
        
        var sql = @"
            UPDATE Users
            SET Email = @Email,
                RoleId = @RoleId,
                IsActive = @IsActive,
                ModifiedAt = @ModifiedAt,
                ModifiedBy = @ModifiedBy
            WHERE UserId = @UserId";
        
        var rowsAffected = await connection.ExecuteAsync(sql, entity);
        
        return rowsAffected > 0;
    }
    

    //Soft delete User and change IsActive to false
    public override async Task<bool> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        
        var sql = @"
            UPDATE Users 
            SET IsActive = 0, 
                ModifiedAt = GETUTCDATE() 
            WHERE UserId = @UserId";
        
        var rowsAffected = await connection.ExecuteAsync(sql, new { UserId = id });
        
        return rowsAffected > 0;
    }
}
