using HRManagementSystem.Application.DTOs.Requests.Designation;
using HRManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRManagementSystem.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HR")] // Only Admin and HR can manage designations


public class DesignationController : ControllerBase
{
    private readonly IDesignationService _designationService;
    private readonly ILogger<DesignationController> _logger;

    public DesignationController(IDesignationService designationService, ILogger<DesignationController> logger)
    {
        _designationService = designationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all designations with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllDesignations(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _designationService.GetAllDesignationsAsync(pageNumber, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting designations");
            return StatusCode(500, new { message = "An error occurred while retrieving designations" });
        }
    }

    
    
    /// <summary>
    /// Get designation by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDesignationById(int id)
    {
        try
        {
            var designation = await _designationService.GetDesignationByIdAsync(id);
            
            if (designation == null)
            {
                return NotFound(new { message = "Designation not found" });
            }

            return Ok(designation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting designation {DesignationId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the designation" });
        }
    }

    
    
    /// <summary>
    /// Create new designation
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateDesignation([FromBody] CreateDesignationRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var designationId = await _designationService.CreateDesignationAsync(request, userId);
            var designation = await _designationService.GetDesignationByIdAsync(designationId);

            return CreatedAtAction(nameof(GetDesignationById), new { id = designationId }, designation);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Designation creation failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating designation");
            return StatusCode(500, new { message = "An error occurred while creating the designation" });
        }
    }

    
    
    /// <summary>
    /// Update existing designation
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDesignation(int id, [FromBody] UpdateDesignationRequest request)
    {
        try
        {
            if (id != request.DesignationId)
            {
                return BadRequest(new { message = "Designation ID mismatch" });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var success = await _designationService.UpdateDesignationAsync(request, userId);

            if (!success)
            {
                return NotFound(new { message = "Designation not found" });
            }

            var designation = await _designationService.GetDesignationByIdAsync(id);
            return Ok(designation);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Designation update failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating designation {DesignationId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the designation" });
        }
    }

    
    
    /// <summary>
    /// Delete designation (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDesignation(int id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var success = await _designationService.DeleteDesignationAsync(id, userId);

            if (!success)
            {
                return NotFound(new { message = "Designation not found" });
            }

            return Ok(new { message = "Designation deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Designation deletion failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting designation {DesignationId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the designation" });
        }
    }
}