using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Application.DTOs.Requests.Designation;
using HRManagementSystem.Application.DTOs.Responses.Designation;

namespace HRManagementSystem.Application.Interfaces.Services;

/// <summary>
///Designation service interface
///Handles business logic for designation operations
/// </summary>
public interface IDesignationService
{
    Task<PagedResult<DesignationResponse>> GetAllDesignationsAsync(int pageNumber, int pageSize);
    Task<DesignationResponse?> GetDesignationByIdAsync(int designationId);
    Task<int> CreateDesignationAsync(CreateDesignationRequest request, int createdBy);
    Task<bool> UpdateDesignationAsync(UpdateDesignationRequest request, int modifiedBy);
    Task<bool> DeleteDesignationAsync(int designationId, int modifiedBy);
}