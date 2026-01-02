using Dapper;
using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Infrastructure.Data;
using System.Data;

namespace HRManagementSystem.Infrastructure.Repositories;

/// <summary>
/// Employee repository implementation using Dapper
/// </summary>
public class EmployeeRepository : BaseRepository<EmployeeProfile>, IEmployeeRepository
{
    public EmployeeRepository(DapperContext context) : base(context) { }

    public override async Task<EmployeeProfile?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("EmployeeId", id);

        var result = await connection.QueryFirstOrDefaultAsync<EmployeeProfile>(
            "sp_GetEmployeeById",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public override async Task<PagedResult<EmployeeProfile>> GetAllAsync(int pageNumber, int pageSize)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("PageNumber", pageNumber);
        parameters.Add("PageSize", pageSize);
        parameters.Add("DepartmentId", null);
        parameters.Add("DesignationId", null);

        using var multi = await connection.QueryMultipleAsync(
            "sp_GetAllEmployees",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        var totalRecords = await multi.ReadSingleAsync<int>();
        var employees = (await multi.ReadAsync<EmployeeProfile>()).ToList();

        return new PagedResult<EmployeeProfile>
        {
            Items = employees,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }

    public override async Task<int> CreateAsync(EmployeeProfile entity)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("UserId", entity.UserId);
        parameters.Add("EmployeeCode", entity.EmployeeCode);
        parameters.Add("FirstName", entity.FirstName);
        parameters.Add("LastName", entity.LastName);
        parameters.Add("DepartmentId", entity.DepartmentId);
        parameters.Add("DesignationId", entity.DesignationId);
        parameters.Add("ManagerId", entity.ManagerId);
        parameters.Add("DateOfBirth", entity.DateOfBirth);
        parameters.Add("Gender", entity.Gender);
        parameters.Add("Phone", entity.Phone);
        parameters.Add("Address", entity.Address);
        parameters.Add("JoinDate", entity.JoinDate);
        parameters.Add("CreatedBy", entity.CreatedBy);

        var result = await connection.QuerySingleAsync<int>(
            "sp_CreateEmployee",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public override async Task<bool> UpdateAsync(EmployeeProfile entity)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("EmployeeId", entity.EmployeeId);
        parameters.Add("EmployeeCode", entity.EmployeeCode);
        parameters.Add("FirstName", entity.FirstName);
        parameters.Add("LastName", entity.LastName);
        parameters.Add("DepartmentId", entity.DepartmentId);
        parameters.Add("DesignationId", entity.DesignationId);
        parameters.Add("ManagerId", entity.ManagerId);
        parameters.Add("DateOfBirth", entity.DateOfBirth);
        parameters.Add("Gender", entity.Gender);
        parameters.Add("Phone", entity.Phone);
        parameters.Add("Address", entity.Address);
        parameters.Add("JoinDate", entity.JoinDate);
        parameters.Add("ResignationDate", entity.ResignationDate);
        parameters.Add("ModifiedBy", entity.ModifiedBy);

        await connection.ExecuteAsync(
            "sp_UpdateEmployee",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return true;
    }

    public override async Task<bool> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("EmployeeId", id);
        parameters.Add("ModifiedBy", 1); // TODO: Get from current user context

        await connection.ExecuteAsync(
            "sp_DeleteEmployee",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return true;
    }

    public async Task<EmployeeProfile?> GetByUserIdAsync(int userId)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId);

        var result = await connection.QueryFirstOrDefaultAsync<EmployeeProfile>(
            "sp_GetEmployeeByUserId",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<bool> EmployeeCodeExistsAsync(string employeeCode, int? excludeEmployeeId = null)
    {
        using var connection = _context.CreateConnection();
        
        var sql = excludeEmployeeId.HasValue
            ? "SELECT CASE WHEN EXISTS (SELECT 1 FROM EmployeeProfiles WHERE EmployeeCode = @EmployeeCode AND EmployeeId != @ExcludeEmployeeId) THEN 1 ELSE 0 END"
            : "SELECT CASE WHEN EXISTS (SELECT 1 FROM EmployeeProfiles WHERE EmployeeCode = @EmployeeCode) THEN 1 ELSE 0 END";

        var exists = await connection.ExecuteScalarAsync<bool>(sql, new 
        { 
            EmployeeCode = employeeCode,
            ExcludeEmployeeId = excludeEmployeeId
        });

        return exists;
    }

    public async Task<bool> UserHasEmployeeProfileAsync(int userId)
    {
        using var connection = _context.CreateConnection();
        
        var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM EmployeeProfiles WHERE UserId = @UserId) THEN 1 ELSE 0 END";
        var exists = await connection.ExecuteScalarAsync<bool>(sql, new { UserId = userId });

        return exists;
    }

    public async Task<bool> IsManagerForOthersAsync(int employeeId)
    {
        using var connection = _context.CreateConnection();
        
        var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM EmployeeProfiles WHERE ManagerId = @EmployeeId AND IsActive = 1) THEN 1 ELSE 0 END";
        var isManager = await connection.ExecuteScalarAsync<bool>(sql, new { EmployeeId = employeeId });

        return isManager;
    }
}