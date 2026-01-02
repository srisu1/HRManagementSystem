using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Application.DTOs.Requests.Employee;
using HRManagementSystem.Application.DTOs.Responses.Employee;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;

namespace HRManagementSystem.Application.Services;

/// <summary>
///Employee service implementation
///Contains business logic for employee management
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    
    
    public async Task<PagedResult<EmployeeResponse>> GetAllEmployeesAsync(int pageNumber, int pageSize, int? departmentId = null, int? designationId = null)
    {
        var pagedEmployees = await _employeeRepository.GetAllAsync(pageNumber, pageSize);
        
        //Map EmployeeProfile entities to EmployeeResponse DTOs
        var employeeResponses = pagedEmployees.Items.Select(e => MapToResponse(e)).ToList();
        
        return new PagedResult<EmployeeResponse>
        {
            Items = employeeResponses,
            PageNumber = pagedEmployees.PageNumber,
            PageSize = pagedEmployees.PageSize,
            TotalRecords = pagedEmployees.TotalRecords
        };
    }

    
    
    public async Task<EmployeeResponse?> GetEmployeeByIdAsync(int employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        
        if (employee == null)
            return null;
            
        return MapToResponse(employee);
    }

    
    
    public async Task<EmployeeResponse?> GetEmployeeByUserIdAsync(int userId)
    {
        var employee = await _employeeRepository.GetByUserIdAsync(userId);
        
        if (employee == null)
            return null;
            
        return MapToResponse(employee);
    }

    
    
    public async Task<int> CreateEmployeeAsync(CreateEmployeeRequest request, int createdBy)
    {
        //Validate employee code doesn't exist
        if (await _employeeRepository.EmployeeCodeExistsAsync(request.EmployeeCode))
        {
            throw new InvalidOperationException("Employee code already exists");
        }

        //Validate user doesn't already have an employee profile
        if (await _employeeRepository.UserHasEmployeeProfileAsync(request.UserId))
        {
            throw new InvalidOperationException("User already has an employee profile");
        }

        var employee = new Domain.Entities.EmployeeProfile
        {
            UserId = request.UserId,
            EmployeeCode = request.EmployeeCode,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DepartmentId = request.DepartmentId,
            DesignationId = request.DesignationId,
            ManagerId = request.ManagerId,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Phone = request.Phone,
            Address = request.Address,
            JoinDate = request.JoinDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        return await _employeeRepository.CreateAsync(employee);
    }

    public async Task<bool> UpdateEmployeeAsync(UpdateEmployeeRequest request, int modifiedBy)
    {
        //Validate employee exists
        var existingEmployee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
        if (existingEmployee == null)
        {
            throw new InvalidOperationException("Employee not found");
        }

        //Validate employee code
        if (await _employeeRepository.EmployeeCodeExistsAsync(request.EmployeeCode, request.EmployeeId))
        {
            throw new InvalidOperationException("Employee code already exists");
        }

        //Prevent employee from being their own manager
        if (request.ManagerId == request.EmployeeId)
        {
            throw new InvalidOperationException("Employee cannot be their own manager");
        }

        var employee = new Domain.Entities.EmployeeProfile
        {
            EmployeeId = request.EmployeeId,
            UserId = existingEmployee.UserId, // Keep the original UserId
            EmployeeCode = request.EmployeeCode,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DepartmentId = request.DepartmentId,
            DesignationId = request.DesignationId,
            ManagerId = request.ManagerId,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Phone = request.Phone,
            Address = request.Address,
            JoinDate = request.JoinDate,
            ResignationDate = request.ResignationDate,
            ModifiedAt = DateTime.UtcNow,
            ModifiedBy = modifiedBy
        };

        return await _employeeRepository.UpdateAsync(employee);
    }

    
    
    public async Task<bool> DeleteEmployeeAsync(int employeeId, int modifiedBy)
    {
        //Check if employee is a manager for others
        if (await _employeeRepository.IsManagerForOthersAsync(employeeId))
        {
            throw new InvalidOperationException("Cannot delete employee who is managing other employees");
        }

        return await _employeeRepository.DeleteAsync(employeeId);
    }

    //Helper method to map EmployeeProfile entity to EmployeeResponse DTO
    private EmployeeResponse MapToResponse(Domain.Entities.EmployeeProfile employee)
    {
        return new EmployeeResponse
        {
            EmployeeId = employee.EmployeeId,
            UserId = employee.UserId,
            EmployeeCode = employee.EmployeeCode,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            DepartmentId = employee.DepartmentId,
            DesignationId = employee.DesignationId,
            ManagerId = employee.ManagerId,
            DateOfBirth = employee.DateOfBirth,
            Gender = employee.Gender,
            Phone = employee.Phone,
            Address = employee.Address,
            JoinDate = employee.JoinDate,
            ResignationDate = employee.ResignationDate,
            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt,
            Email = null,
            DepartmentName = null,
            DepartmentCode = null,
            DesignationName = null,
            DesignationLevel = null,
            ManagerName = null
        };
    }
}