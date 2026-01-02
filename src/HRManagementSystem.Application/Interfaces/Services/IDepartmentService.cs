using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Application.DTOs.Requests.Department;
using HRManagementSystem.Application.DTOs.Responses.Department;

namespace HRManagementSystem.Application.Interfaces.Services;

/// <summary>
///Department service interface
///Handles business logic for department operations
/// </summary>

public interface IDepartmentService
{
    Task<PagedResult<DepartmentResponse>> GetAllDepartmentsAsync(int pageNumber, int pageSize, int? branchId = null);
    Task<DepartmentResponse?> GetDepartmentByIdAsync(int departmentId);
    Task<IEnumerable<DepartmentResponse>> GetDepartmentsByBranchAsync(int branchId);
    Task<int> CreateDepartmentAsync(CreateDepartmentRequest request, int createdBy);
    Task<bool> UpdateDepartmentAsync(UpdateDepartmentRequest request, int modifiedBy);
    Task<bool> DeleteDepartmentAsync(int departmentId, int modifiedBy);
}