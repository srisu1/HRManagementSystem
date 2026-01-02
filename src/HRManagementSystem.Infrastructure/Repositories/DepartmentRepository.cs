using Dapper;
using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Application.DTOs.Responses.Department;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Infrastructure.Data;
using System.Data;

namespace HRManagementSystem.Infrastructure.Repositories;

/// <summary>
///Department repository implementation using Dapper
/// </summary>
public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(DapperContext context) : base(context) { }

    public override async Task<Department?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("DepartmentId", id);

        var result = await connection.QueryFirstOrDefaultAsync<Department>(
            "sp_GetDepartmentById",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public override async Task<PagedResult<Department>> GetAllAsync(int pageNumber, int pageSize)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("PageNumber", pageNumber);
        parameters.Add("PageSize", pageSize);

        using var multi = await connection.QueryMultipleAsync(
            "sp_GetAllDepartments",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        var totalRecords = await multi.ReadSingleAsync<int>();
        var departments = (await multi.ReadAsync<Department>()).ToList();

        return new PagedResult<Department>
        {
            Items = departments,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }

    public override async Task<int> CreateAsync(Department entity)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("BranchId", entity.BranchId);
        parameters.Add("DepartmentName", entity.DepartmentName);
        parameters.Add("DepartmentCode", entity.DepartmentCode);
        parameters.Add("ParentDepartmentId", entity.ParentDepartmentId);
        parameters.Add("ManagerId", entity.ManagerId);
        parameters.Add("Description", entity.Description);
        parameters.Add("CreatedBy", entity.CreatedBy);

        var result = await connection.QuerySingleAsync<int>(
            "sp_CreateDepartment",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public override async Task<bool> UpdateAsync(Department entity)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("DepartmentId", entity.DepartmentId);
        parameters.Add("BranchId", entity.BranchId);
        parameters.Add("DepartmentName", entity.DepartmentName);
        parameters.Add("DepartmentCode", entity.DepartmentCode);
        parameters.Add("ParentDepartmentId", entity.ParentDepartmentId);
        parameters.Add("ManagerId", entity.ManagerId);
        parameters.Add("Description", entity.Description);
        parameters.Add("ModifiedBy", entity.ModifiedBy);

        await connection.ExecuteAsync(
            "sp_UpdateDepartment",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return true;
    }

    public override async Task<bool> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("DepartmentId", id);
        parameters.Add("ModifiedBy", 1); // TODO: Get from current user context

        await connection.ExecuteAsync(
            "sp_DeleteDepartment",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return true;
    }

    public async Task<IEnumerable<Department>> GetByBranchIdAsync(int branchId)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("BranchId", branchId);

        var departments = await connection.QueryAsync<Department>(
            "sp_GetDepartmentsByBranch",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return departments;
    }

    public async Task<bool> DepartmentCodeExistsAsync(string departmentCode, int? excludeDepartmentId = null)
    {
        using var connection = _context.CreateConnection();
        
        var sql = excludeDepartmentId.HasValue
            ? "SELECT CASE WHEN EXISTS (SELECT 1 FROM Departments WHERE DepartmentCode = @DepartmentCode AND DepartmentId != @ExcludeDepartmentId) THEN 1 ELSE 0 END"
            : "SELECT CASE WHEN EXISTS (SELECT 1 FROM Departments WHERE DepartmentCode = @DepartmentCode) THEN 1 ELSE 0 END";

        var exists = await connection.ExecuteScalarAsync<bool>(sql, new 
        { 
            DepartmentCode = departmentCode,
            ExcludeDepartmentId = excludeDepartmentId
        });

        return exists;
    }

    public async Task<bool> HasEmployeesAsync(int departmentId)
    {
        using var connection = _context.CreateConnection();
        
        var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM EmployeeProfiles WHERE DepartmentId = @DepartmentId AND IsActive = 1) THEN 1 ELSE 0 END";
        var hasEmployees = await connection.ExecuteScalarAsync<bool>(sql, new { DepartmentId = departmentId });

        return hasEmployees;
    }

    public async Task<bool> HasChildDepartmentsAsync(int departmentId)
    {
        using var connection = _context.CreateConnection();
        
        var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Departments WHERE ParentDepartmentId = @DepartmentId AND IsActive = 1) THEN 1 ELSE 0 END";
        var hasChildren = await connection.ExecuteScalarAsync<bool>(sql, new { DepartmentId = departmentId });

        return hasChildren;
    }
}