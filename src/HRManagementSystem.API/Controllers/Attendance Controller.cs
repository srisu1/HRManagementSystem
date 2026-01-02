using HRManagementSystem.Application.DTOs.Requests.Attendance;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;



namespace HRManagementSystem.API.Controllers;


[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]


public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<AttendanceController> _logger;

    public AttendanceController(
        IAttendanceService attendanceService,
        IEmployeeRepository employeeRepository,
        ILogger<AttendanceController> logger)
    {
        _attendanceService = attendanceService;
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    /// <summary>
    /// Check ib for all authenticated users
    /// </summary>
    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            // Get employee profile for this user
            var employee = await _employeeRepository.GetByUserIdAsync(userId);
            if (employee == null)
            {
                return NotFound(new { message = "Employee profile not found" });
            }

            var request = new CheckInRequest
            {
                EmployeeId = employee.EmployeeId,
                CheckInTime = DateTime.Now
            };

            var attendanceId = await _attendanceService.CheckInAsync(request);
            var attendance = await _attendanceService.GetTodayAttendanceAsync(employee.EmployeeId);

            return Ok(new
            {
                message = "Checked in successfully",
                attendanceId = attendanceId,
                attendance = attendance
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Check-in failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during check-in");
            return StatusCode(500, new { message = "An error occurred during check-in" });
        }
    }

    /// <summary>
    /// Check out for all authenticated users
    /// </summary>
    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            //Get employee profile for this user
            var employee = await _employeeRepository.GetByUserIdAsync(userId);
            if (employee == null)
            {
                return NotFound(new { message = "Employee profile not found" });
            }

            var request = new CheckOutRequest
            {
                EmployeeId = employee.EmployeeId,
                CheckOutTime = DateTime.Now
            };

            var success = await _attendanceService.CheckOutAsync(request);
            var attendance = await _attendanceService.GetTodayAttendanceAsync(employee.EmployeeId);

            return Ok(new
            {
                message = "Checked out successfully",
                attendance = attendance
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Check-out failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during check-out");
            return StatusCode(500, new { message = "An error occurred during check-out" });
        }
    }

    /// <summary>
    /// Get todays attendance status for all authenticated users
    /// </summary>
    [HttpGet("today")]
    public async Task<IActionResult> GetTodayAttendance()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var employee = await _employeeRepository.GetByUserIdAsync(userId);
            if (employee == null)
            {
                return NotFound(new { message = "Employee profile not found" });
            }

            var attendance = await _attendanceService.GetTodayAttendanceAsync(employee.EmployeeId);
            
            if (attendance == null)
            {
                return Ok(new { message = "Not checked in yet", attendance = (object?)null });
            }

            return Ok(attendance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's attendance");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    ///Get my attendance history for all authenticated users
    /// </summary>
    [HttpGet("my-history")]
    public async Task<IActionResult> GetMyAttendanceHistory(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var employee = await _employeeRepository.GetByUserIdAsync(userId);
            if (employee == null)
            {
                return NotFound(new { message = "Employee profile not found" });
            }

            var result = await _attendanceService.GetMyAttendanceHistoryAsync(
                employee.EmployeeId, startDate, endDate, pageNumber, pageSize);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting attendance history");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    ///Get my attendance summary for a month
    /// </summary>
    [HttpGet("my-summary")]
    public async Task<IActionResult> GetMyAttendanceSummary(
        [FromQuery] int month,
        [FromQuery] int year)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var employee = await _employeeRepository.GetByUserIdAsync(userId);
            if (employee == null)
            {
                return NotFound(new { message = "Employee profile not found" });
            }

            var summary = await _attendanceService.GetAttendanceSummaryAsync(employee.EmployeeId, month, year);

            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting attendance summary");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    ///Get all attendance for a specific date (access to Adminor HR only)
    /// </summary>
    [HttpGet("by-date")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> GetAttendanceByDate(
        [FromQuery] DateTime date,
        [FromQuery] int? departmentId = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _attendanceService.GetAttendanceByDateAsync(date, departmentId, pageNumber, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting attendance by date");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Get team attendance (allow access for managers)
    /// </summary>
    [HttpGet("team")]
    public async Task<IActionResult> GetTeamAttendance(
        [FromQuery] DateTime? date = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var employee = await _employeeRepository.GetByUserIdAsync(userId);
            if (employee == null)
            {
                return NotFound(new { message = "Employee profile not found" });
            }

            var attendanceDate = date ?? DateTime.Today;
            var result = await _attendanceService.GetTeamAttendanceAsync(employee.EmployeeId, attendanceDate, pageNumber, pageSize);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting team attendance");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}