using HRManagementSystem.Application.DTOs.Common;
using HRManagementSystem.Domain.Entities;

namespace HRManagementSystem.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Designation entity
/// </summary>
public interface IDesignationRepository : IBaseRepository<Designation>
{
    Task<bool> DesignationCodeExistsAsync(string designationCode, int? excludeDesignationId = null);
    Task<bool> HasEmployeesAsync(int designationId);
}