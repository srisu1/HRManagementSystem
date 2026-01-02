using Dapper;
using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Application.DTOs.Responses.Attendance;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Infrastructure.Data;
using System.Data;

namespace HRManagementSystem.Infrastructure.Repositories;

/// <summary>
///repository implementation using Dapper
/// </summary>
public class AttendanceRepository : BaseRepository<Attendance>, IAttendanceRepository
{
    public AttendanceRepository(DapperContext context) : base(context) { }

    public async Task<int> CheckInAsync(int employeeId, DateTime checkInTime)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("EmployeeId", employeeId);
        parameters.Add("AttendanceDate", checkInTime.Date);
        parameters.Add("CheckInTime", checkInTime);

        var result = await connection.QuerySingleAsync<int>(
            "sp_CheckIn",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<bool> CheckOutAsync(int employeeId, DateTime checkOutTime)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("EmployeeId", employeeId);
        parameters.Add("AttendanceDate", checkOutTime.Date);
        parameters.Add("CheckOutTime", checkOutTime);

        await connection.ExecuteAsync(
            "sp_CheckOut",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return true;
    }

    public async Task<Attendance?> GetTodayAttendanceAsync(int employeeId)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("EmployeeId", employeeId);

        var result = await connection.QueryFirstOrDefaultAsync<Attendance>(
            "sp_GetTodayAttendance",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<PagedResult<Attendance>> GetAttendanceByEmployeeAsync(int employeeId, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("EmployeeId", employeeId);
        parameters.Add("StartDate", startDate);
        parameters.Add("EndDate", endDate);
        parameters.Add("PageNumber", pageNumber);
        parameters.Add("PageSize", pageSize);

        using var multi = await connection.QueryMultipleAsync(
            "sp_GetAttendanceByEmployee",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        var totalRecords = await multi.ReadSingleAsync<int>();
        var attendance = (await multi.ReadAsync<Attendance>()).ToList();

        return new PagedResult<Attendance>
        {
            Items = attendance,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }

    public async Task<PagedResult<Attendance>> GetAttendanceByDateAsync(DateTime date, int? departmentId, int pageNumber, int pageSize)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("AttendanceDate", date.Date);
        parameters.Add("DepartmentId", departmentId);
        parameters.Add("PageNumber", pageNumber);
        parameters.Add("PageSize", pageSize);

        using var multi = await connection.QueryMultipleAsync(
            "sp_GetAttendanceByDate",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        var totalRecords = await multi.ReadSingleAsync<int>();
        var attendance = (await multi.ReadAsync<Attendance>()).ToList();

        return new PagedResult<Attendance>
        {
            Items = attendance,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }

    public async Task<PagedResult<Attendance>> GetTeamAttendanceAsync(int managerId, DateTime date, int pageNumber, int pageSize)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("ManagerId", managerId);
        parameters.Add("AttendanceDate", date.Date);
        parameters.Add("PageNumber", pageNumber);
        parameters.Add("PageSize", pageSize);

        using var multi = await connection.QueryMultipleAsync(
            "sp_GetTeamAttendance",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        var totalRecords = await multi.ReadSingleAsync<int>();
        var attendance = (await multi.ReadAsync<Attendance>()).ToList();

        return new PagedResult<Attendance>
        {
            Items = attendance,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }

    public async Task<AttendanceSummaryResponse> GetAttendanceSummaryAsync(int employeeId, int month, int year)
    {
        using var connection = _context.CreateConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("EmployeeId", employeeId);
        parameters.Add("Month", month);
        parameters.Add("Year", year);

        var result = await connection.QuerySingleOrDefaultAsync<AttendanceSummaryResponse>(
            "sp_GetAttendanceSummary",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        if (result != null)
        {
            result.Month = month;
            result.Year = year;
        }

        return result ?? new AttendanceSummaryResponse { Month = month, Year = year };
    }

    public override async Task<Attendance?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        
        var sql = "SELECT * FROM Attendance WHERE AttendanceId = @AttendanceId";
        var result = await connection.QueryFirstOrDefaultAsync<Attendance>(sql, new { AttendanceId = id });

        return result;
    }

    public override async Task<PagedResult<Attendance>> GetAllAsync(int pageNumber, int pageSize)
    {
        using var connection = _context.CreateConnection();
        
        var offset = (pageNumber - 1) * pageSize;
        
        var sql = @"
            SELECT COUNT(*) FROM Attendance;
            
            SELECT * FROM Attendance
            ORDER BY AttendanceDate DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

        using var multi = await connection.QueryMultipleAsync(sql, new { Offset = offset, PageSize = pageSize });

        var totalRecords = await multi.ReadSingleAsync<int>();
        var attendance = (await multi.ReadAsync<Attendance>()).ToList();

        return new PagedResult<Attendance>
        {
            Items = attendance,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }

    public override async Task<int> CreateAsync(Attendance entity)
    {
        throw new NotImplementedException("Use CheckInAsync instead");
    }

    public override async Task<bool> UpdateAsync(Attendance entity)
    {
        throw new NotImplementedException("Use CheckOutAsync instead");
    }

    public override async Task<bool> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        
        var sql = "DELETE FROM Attendance WHERE AttendanceId = @AttendanceId";
        var rowsAffected = await connection.ExecuteAsync(sql, new { AttendanceId = id });

        return rowsAffected > 0;
    }
}