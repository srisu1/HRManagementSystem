namespace HRManagementSystem.Domain.Enums;

/// <summary>
///User roles in the system
/// </summary>
public enum UserRoleEnum
{
    Admin = 1, //Admin has access
    HR = 2, //Hr can manage employees
    Accountant = 3, //Accountant has finance access
    Staff = 4 //Staff is a basic user
}