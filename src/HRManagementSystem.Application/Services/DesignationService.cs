using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Application.DTOs.Requests.Designation;
using HRManagementSystem.Application.DTOs.Responses.Designation;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;

namespace HRManagementSystem.Application.Services;

/// <summary>
///Designation service implementation
///Contains business logic for designation management
/// </summary>


public class DesignationService : IDesignationService
{
    private readonly IDesignationRepository _designationRepository;

    public DesignationService(IDesignationRepository designationRepository)
    {
        _designationRepository = designationRepository;
    }

    
    
    public async Task<PagedResult<DesignationResponse>> GetAllDesignationsAsync(int pageNumber, int pageSize)
    {
        var pagedDesignations = await _designationRepository.GetAllAsync(pageNumber, pageSize);
        
        //Map Designation entities to DesignationResponse DTOs
        var designationResponses = pagedDesignations.Items.Select(d => MapToResponse(d)).ToList();
        
        return new PagedResult<DesignationResponse>
        {
            Items = designationResponses,
            PageNumber = pagedDesignations.PageNumber,
            PageSize = pagedDesignations.PageSize,
            TotalRecords = pagedDesignations.TotalRecords
        };
    }

    
    
    public async Task<DesignationResponse?> GetDesignationByIdAsync(int designationId)
    {
        var designation = await _designationRepository.GetByIdAsync(designationId);
        
        if (designation == null)
            return null;
            
        return MapToResponse(designation);
    }

    
    
    public async Task<int> CreateDesignationAsync(CreateDesignationRequest request, int createdBy)
    {
        // Validate designation code doesn't exist
        if (await _designationRepository.DesignationCodeExistsAsync(request.DesignationCode))
        {
            throw new InvalidOperationException("Designation code already exists");
        }

        var designation = new Domain.Entities.Designation
        {
            DesignationName = request.DesignationName,
            DesignationCode = request.DesignationCode,
            Level = request.Level,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        return await _designationRepository.CreateAsync(designation);
    }

    
    
    public async Task<bool> UpdateDesignationAsync(UpdateDesignationRequest request, int modifiedBy)
    {
        //Validate designation exists
        var existingDesignation = await _designationRepository.GetByIdAsync(request.DesignationId);
        if (existingDesignation == null)
        {
            throw new InvalidOperationException("Designation not found");
        }

        //Validate designation code
        if (await _designationRepository.DesignationCodeExistsAsync(request.DesignationCode, request.DesignationId))
        {
            throw new InvalidOperationException("Designation code already exists");
        }

        var designation = new Domain.Entities.Designation
        {
            DesignationId = request.DesignationId,
            DesignationName = request.DesignationName,
            DesignationCode = request.DesignationCode,
            Level = request.Level,
            Description = request.Description,
            ModifiedAt = DateTime.UtcNow,
            ModifiedBy = modifiedBy
        };

        return await _designationRepository.UpdateAsync(designation);
    }

   
    
    public async Task<bool> DeleteDesignationAsync(int designationId, int modifiedBy)
    {
        //Check if designation has employees
        if (await _designationRepository.HasEmployeesAsync(designationId))
        {
            throw new InvalidOperationException("Cannot delete designation with active employees");
        }

        return await _designationRepository.DeleteAsync(designationId);
    }

    
    //Helper to map Designation entity to DesignationResponse DTO
    private DesignationResponse MapToResponse(Domain.Entities.Designation designation)
    {
        return new DesignationResponse
        {
            DesignationId = designation.DesignationId,
            DesignationName = designation.DesignationName,
            DesignationCode = designation.DesignationCode,
            Level = designation.Level,
            Description = designation.Description,
            IsActive = designation.IsActive,
            CreatedAt = designation.CreatedAt
        };
    }
}