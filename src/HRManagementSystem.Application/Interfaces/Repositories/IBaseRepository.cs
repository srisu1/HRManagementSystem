using HRManagementSystem.Application.DTOs.Common;

namespace HRManagementSystem.Application.Interfaces.Repositories;

/// <summary>
///This is the base repository interface
///All specific repositories will inherit from this
///T is the entity type (User, Employee, etc)
/// </summary>


public interface IBaseRepository<T> where T : class
{
   
    
    //Get a single record by ID
    Task<T?> GetByIdAsync(int id);
    

    //Get all records with pagination
    Task<PagedResult<T>> GetAllAsync(int pageNumber, int pageSize);
    
  
    //Create a new record
    //Returns the new ID
    Task<int> CreateAsync(T entity);
    

    //Update an existing record
    //Returns true if successful
    Task<bool> UpdateAsync(T entity);
    

    //Delete a record (usually soft delete)
    //Returns true if successful
    Task<bool> DeleteAsync(int id);
}