using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Application.DTOs.Requests.Employee;
using HRManagementSystem.Application.DTOs.Responses.Employee;

namespace HRManagementSystem.Application.Interfaces.Services;

/// <summary>
///Employee service interface
///Handles business logic for employee operations
/// </summary>
public interface IEmployeeService
{
    Task<PagedResult<EmployeeResponse>> GetAllEmployeesAsync(int pageNumber, int pageSize, int? departmentId = null, int? designationId = null);
    Task<EmployeeResponse?> GetEmployeeByIdAsync(int employeeId);
    Task<EmployeeResponse?> GetEmployeeByUserIdAsync(int userId);
    Task<int> CreateEmployeeAsync(CreateEmployeeRequest request, int createdBy);
    Task<bool> UpdateEmployeeAsync(UpdateEmployeeRequest request, int modifiedBy);
    Task<bool> DeleteEmployeeAsync(int employeeId, int modifiedBy);
}