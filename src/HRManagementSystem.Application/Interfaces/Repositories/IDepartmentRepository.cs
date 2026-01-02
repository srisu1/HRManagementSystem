using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Domain.Entities;

namespace HRManagementSystem.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Department entity
/// </summary>

public interface IDepartmentRepository : IBaseRepository<Department>
{
    Task<IEnumerable<Department>> GetByBranchIdAsync(int branchId);
    Task<bool> DepartmentCodeExistsAsync(string departmentCode, int? excludeDepartmentId = null);
    Task<bool> HasEmployeesAsync(int departmentId);
    Task<bool> HasChildDepartmentsAsync(int departmentId);
}