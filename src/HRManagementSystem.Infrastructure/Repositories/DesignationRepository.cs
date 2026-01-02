using Dapper;
using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Infrastructure.Data;
using System.Data;

namespace HRManagementSystem.Infrastructure.Repositories;

/// <summary>
/// Designation repository implementation using Dapper
/// </summary>
public class DesignationRepository : BaseRepository<Designation>, IDesignationRepository
{
    public DesignationRepository(DapperContext context) : base(context) { }

    public override async Task<Designation?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("DesignationId", id);

        var result = await connection.QueryFirstOrDefaultAsync<Designation>(
            "sp_GetDesignationById",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public override async Task<PagedResult<Designation>> GetAllAsync(int pageNumber, int pageSize)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("PageNumber", pageNumber);
        parameters.Add("PageSize", pageSize);

        using var multi = await connection.QueryMultipleAsync(
            "sp_GetAllDesignations",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        var totalRecords = await multi.ReadSingleAsync<int>();
        var designations = (await multi.ReadAsync<Designation>()).ToList();

        return new PagedResult<Designation>
        {
            Items = designations,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }

    public override async Task<int> CreateAsync(Designation entity)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("DesignationName", entity.DesignationName);
        parameters.Add("DesignationCode", entity.DesignationCode);
        parameters.Add("Level", entity.Level);
        parameters.Add("Description", entity.Description);
        parameters.Add("CreatedBy", entity.CreatedBy);

        var result = await connection.QuerySingleAsync<int>(
            "sp_CreateDesignation",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public override async Task<bool> UpdateAsync(Designation entity)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("DesignationId", entity.DesignationId);
        parameters.Add("DesignationName", entity.DesignationName);
        parameters.Add("DesignationCode", entity.DesignationCode);
        parameters.Add("Level", entity.Level);
        parameters.Add("Description", entity.Description);
        parameters.Add("ModifiedBy", entity.ModifiedBy);

        await connection.ExecuteAsync(
            "sp_UpdateDesignation",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return true;
    }

    public override async Task<bool> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("DesignationId", id);
        parameters.Add("ModifiedBy", 1); // TODO: Get from current user context

        await connection.ExecuteAsync(
            "sp_DeleteDesignation",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return true;
    }

    public async Task<bool> DesignationCodeExistsAsync(string designationCode, int? excludeDesignationId = null)
    {
        using var connection = _context.CreateConnection();
        
        var sql = excludeDesignationId.HasValue
            ? "SELECT CASE WHEN EXISTS (SELECT 1 FROM Designations WHERE DesignationCode = @DesignationCode AND DesignationId != @ExcludeDesignationId) THEN 1 ELSE 0 END"
            : "SELECT CASE WHEN EXISTS (SELECT 1 FROM Designations WHERE DesignationCode = @DesignationCode) THEN 1 ELSE 0 END";

        var exists = await connection.ExecuteScalarAsync<bool>(sql, new 
        { 
            DesignationCode = designationCode,
            ExcludeDesignationId = excludeDesignationId
        });

        return exists;
    }

    public async Task<bool> HasEmployeesAsync(int designationId)
    {
        using var connection = _context.CreateConnection();
        
        var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM EmployeeProfiles WHERE DesignationId = @DesignationId AND IsActive = 1) THEN 1 ELSE 0 END";
        var hasEmployees = await connection.ExecuteScalarAsync<bool>(sql, new { DesignationId = designationId });

        return hasEmployees;
    }
}