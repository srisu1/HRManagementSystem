using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Domain.Entities;

namespace HRManagementSystem.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for EmployeeProfile entity
/// </summary>
public interface IEmployeeRepository : IBaseRepository<EmployeeProfile>
{
    Task<EmployeeProfile?> GetByUserIdAsync(int userId);
    Task<bool> EmployeeCodeExistsAsync(string employeeCode, int? excludeEmployeeId = null);
    Task<bool> UserHasEmployeeProfileAsync(int userId);
    Task<bool> IsManagerForOthersAsync(int employeeId);
}