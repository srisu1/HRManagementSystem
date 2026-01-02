using HRManagementSystem.Application.DTOs.Requests.Department;
using HRManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRManagementSystem.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize(Roles = "Admin,HR")] // Only Admin and HR can manage departments
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;
    private readonly ILogger<DepartmentController> _logger;

    public DepartmentController(IDepartmentService departmentService, ILogger<DepartmentController> logger)
    {
        _departmentService = departmentService;
        _logger = logger;
    }

    /// <summary>
    /// Get all departments with pagination
    /// </summary>
    
    [HttpGet]
    public async Task<IActionResult> GetAllDepartments(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] int? branchId = null)
    {
        try
        {
            var result = await _departmentService.GetAllDepartmentsAsync(pageNumber, pageSize, branchId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting departments");
            return StatusCode(500, new { message = "An error occurred while retrieving departments" });
        }
    }

    /// <summary>
    ///Get department by id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDepartmentById(int id)
    {
        try
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            
            if (department == null)
            {
                return NotFound(new { message = "Department not found" });
            }

            return Ok(department);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department {DepartmentId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the department" });
        }
    }

    /// <summary>
    /// Get departments by branch
    /// </summary>
    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetDepartmentsByBranch(int branchId)
    {
        try
        {
            var departments = await _departmentService.GetDepartmentsByBranchAsync(branchId);
            return Ok(departments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting departments for branch {BranchId}", branchId);
            return StatusCode(500, new { message = "An error occurred while retrieving departments" });
        }
    }

    
    
    /// <summary>
    /// Create new department
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var departmentId = await _departmentService.CreateDepartmentAsync(request, userId);
            var department = await _departmentService.GetDepartmentByIdAsync(departmentId);

            return CreatedAtAction(nameof(GetDepartmentById), new { id = departmentId }, department);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Department creation failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating department");
            return StatusCode(500, new { message = "An error occurred while creating the department" });
        }
    }

    
    /// <summary>
    /// Update existing department
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentRequest request)
    {
        try
        {
            if (id != request.DepartmentId)
            {
                return BadRequest(new { message = "Department ID mismatch" });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var success = await _departmentService.UpdateDepartmentAsync(request, userId);

            if (!success)
            {
                return NotFound(new { message = "Department not found" });
            }

            var department = await _departmentService.GetDepartmentByIdAsync(id);
            return Ok(department);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Department update failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating department {DepartmentId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the department" });
        }
    }

    
    /// <summary>
    /// Delete department (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var success = await _departmentService.DeleteDepartmentAsync(id, userId);

            if (!success)
            {
                return NotFound(new { message = "Department not found" });
            }

            return Ok(new { message = "Department deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Department deletion failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting department {DepartmentId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the department" });
        }
    }
}