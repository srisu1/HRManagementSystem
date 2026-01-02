using HRManagementSystem.Application.DTOs.Requests.Employee;
using HRManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRManagementSystem.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]


public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeeController> _logger;

    public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    
    
    /// <summary>
    /// Get all employees with pagination (Admin/HR only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> GetAllEmployees(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] int? departmentId = null,
        [FromQuery] int? designationId = null)
    {
        try
        {
            var result = await _employeeService.GetAllEmployeesAsync(pageNumber, pageSize, departmentId, designationId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employees");
            return StatusCode(500, new { message = "An error occurred while retrieving employees" });
        }
    }

    
    
    /// <summary>
    /// Get employee by ID (Admin/HR only)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            
            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }

            return Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employee {EmployeeId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the employee" });
        }
    }

    
    
    /// <summary>
    /// Get own employee profile (All authenticated users)
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
            
            if (employee == null)
            {
                return NotFound(new { message = "Employee profile not found" });
            }

            return Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting own employee profile");
            return StatusCode(500, new { message = "An error occurred while retrieving your profile" });
        }
    }

    
    
    /// <summary>
    /// Create new employee (Admin/HR only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var employeeId = await _employeeService.CreateEmployeeAsync(request, userId);
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);

            return CreatedAtAction(nameof(GetEmployeeById), new { id = employeeId }, employee);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Employee creation failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            return StatusCode(500, new { message = "An error occurred while creating the employee" });
        }
    }

    
    
    /// <summary>
    /// Update existing employee (Admin/HR only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeRequest request)
    {
        try
        {
            if (id != request.EmployeeId)
            {
                return BadRequest(new { message = "Employee ID mismatch" });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var success = await _employeeService.UpdateEmployeeAsync(request, userId);

            if (!success)
            {
                return NotFound(new { message = "Employee not found" });
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            return Ok(employee);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Employee update failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee {EmployeeId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the employee" });
        }
    }

    
    
    /// <summary>
    /// Delete employee (Admin/HR only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var success = await _employeeService.DeleteEmployeeAsync(id, userId);

            if (!success)
            {
                return NotFound(new { message = "Employee not found" });
            }

            return Ok(new { message = "Employee deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Employee deletion failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting employee {EmployeeId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the employee" });
        }
    }
}