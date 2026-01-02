using Dapper;

using HRManagementSystem.Application.DTOs.Common;

using HRManagementSystem.Application.Interfaces.Repositories;

using HRManagementSystem.Infrastructure.Data;

using System.Data;

namespace HRManagementSystem.Infrastructure.Repositories;

/// <summary>
///This is a base repository with CRUD operations
///All specific repositories inherit from this
/// </summary>

public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly DapperContext _context;

    protected BaseRepository(DapperContext context)
    {
        _context = context;
    }

    //These are abstract  child classes must implement them
    public abstract Task<T?> GetByIdAsync(int id);
    public abstract Task<PagedResult<T>> GetAllAsync(int pageNumber, int pageSize);
    public abstract Task<int> CreateAsync(T entity);
    public abstract Task<bool> UpdateAsync(T entity);
    public abstract Task<bool> DeleteAsync(int id);


    //It is a helper method to execute stored procedures and returns a single result
    protected async Task<TResult?> ExecuteStoredProcedureAsync<TResult>(
        string storedProcedure,
        object? parameters = null)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<TResult>(
            storedProcedure,
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }


    //It is a helper method to execute stored procedures and return multiple results
    protected async Task<IEnumerable<TResult>> ExecuteStoredProcedureListAsync<TResult>(
        string storedProcedure,
        object? parameters = null)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<TResult>(
            storedProcedure,
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }


    //It is a helper method to execute stored procedures with no result and used for INSERT, UPDATE, DELETE
    protected async Task<int> ExecuteStoredProcedureNonQueryAsync(
        string storedProcedure,
        object? parameters = null)
    {
        using var connection = _context.CreateConnection();

        return await connection.ExecuteAsync(
            storedProcedure,
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }


    //It is a helper method for raw SQL queries (when stored proc doesn't exist)
    protected async Task<IEnumerable<TResult>> QueryAsync<TResult>(
        string sql,
        object? parameters = null)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<TResult>(sql, parameters);
    }


    //It is a helper method for raw SQL single result
    protected async Task<TResult?> QueryFirstOrDefaultAsync<TResult>(
        string sql,
        object? parameters = null)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<TResult>(sql, parameters);
    }
}