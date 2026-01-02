using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Application.DTOs.Requests.Department;
using HRManagementSystem.Application.DTOs.Responses.Department;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;

namespace HRManagementSystem.Application.Services;

/// <summary>
///Department service implementation
///Contains business logic for department management
/// </summary>


public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    
    public async Task<PagedResult<DepartmentResponse>> GetAllDepartmentsAsync(int pageNumber, int pageSize, int? branchId = null)
    {
        var pagedDepartments = await _departmentRepository.GetAllAsync(pageNumber, pageSize);
        
        //map Department entities to DepartmentResponse DTOs
        var departmentResponses = pagedDepartments.Items.Select(d => MapToResponse(d)).ToList();
        
        return new PagedResult<DepartmentResponse>
        {
            Items = departmentResponses,
            PageNumber = pagedDepartments.PageNumber,
            PageSize = pagedDepartments.PageSize,
            TotalRecords = pagedDepartments.TotalRecords
        };
    }

    
    
    public async Task<DepartmentResponse?> GetDepartmentByIdAsync(int departmentId)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId);
        
        if (department == null)
            return null;
            
        return MapToResponse(department);
    }

    
    
    public async Task<IEnumerable<DepartmentResponse>> GetDepartmentsByBranchAsync(int branchId)
    {
        var departments = await _departmentRepository.GetByBranchIdAsync(branchId);
        return departments.Select(d => MapToResponse(d));
    }

    
    
    public async Task<int> CreateDepartmentAsync(CreateDepartmentRequest request, int createdBy)
    {
        //Validate department code doesn't exist
        if (await _departmentRepository.DepartmentCodeExistsAsync(request.DepartmentCode))
        {
            throw new InvalidOperationException("Department code already exists");
        }

        var department = new Domain.Entities.Department
        {
            BranchId = request.BranchId,
            DepartmentName = request.DepartmentName,
            DepartmentCode = request.DepartmentCode,
            ParentDepartmentId = request.ParentDepartmentId,
            ManagerId = request.ManagerId,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        return await _departmentRepository.CreateAsync(department);
    }

    
    
    public async Task<bool> UpdateDepartmentAsync(UpdateDepartmentRequest request, int modifiedBy)
    {
        //Validate department exists
        var existingDepartment = await _departmentRepository.GetByIdAsync(request.DepartmentId);
        if (existingDepartment == null)
        {
            throw new InvalidOperationException("Department not found");
        }

        //Validate department code
        if (await _departmentRepository.DepartmentCodeExistsAsync(request.DepartmentCode, request.DepartmentId))
        {
            throw new InvalidOperationException("Department code already exists");
        }

        //Prevent circular parent reference
        if (request.ParentDepartmentId == request.DepartmentId)
        {
            throw new InvalidOperationException("Department cannot be its own parent");
        }

        var department = new Domain.Entities.Department
        {
            DepartmentId = request.DepartmentId,
            BranchId = request.BranchId,
            DepartmentName = request.DepartmentName,
            DepartmentCode = request.DepartmentCode,
            ParentDepartmentId = request.ParentDepartmentId,
            ManagerId = request.ManagerId,
            Description = request.Description,
            ModifiedAt = DateTime.UtcNow,
            ModifiedBy = modifiedBy
        };

        return await _departmentRepository.UpdateAsync(department);
    }

    
    
    public async Task<bool> DeleteDepartmentAsync(int departmentId, int modifiedBy)
    {
        //Check if department has employees
        if (await _departmentRepository.HasEmployeesAsync(departmentId))
        {
            throw new InvalidOperationException("Cannot delete department with active employees");
        }

        //Check if department has child departments
        if (await _departmentRepository.HasChildDepartmentsAsync(departmentId))
        {
            throw new InvalidOperationException("Cannot delete department with active sub-departments");
        }

        return await _departmentRepository.DeleteAsync(departmentId);
    }

    //Helper method to map Department entity to DepartmentResponse DTO
    
    
    private DepartmentResponse MapToResponse(Domain.Entities.Department department)
    {
        return new DepartmentResponse
        {
            DepartmentId = department.DepartmentId,
            BranchId = department.BranchId,
            DepartmentName = department.DepartmentName,
            DepartmentCode = department.DepartmentCode,
            ParentDepartmentId = department.ParentDepartmentId,
            ManagerId = department.ManagerId,
            Description = department.Description,
            IsActive = department.IsActive,
            CreatedAt = department.CreatedAt,
            BranchName = null,
            BranchCode = null,
            ParentDepartmentName = null,
            ManagerName = null
        };
    }
}