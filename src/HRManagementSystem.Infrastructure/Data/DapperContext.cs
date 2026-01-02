using Microsoft.Data.SqlClient;

using Microsoft.Extensions.Configuration;

using System.Data;

namespace HRManagementSystem.Infrastructure.Data;

/// <summary>
///Dapper Context creates database connections
///Its like a phone line to the database
/// </summary>


public class DapperContext
{
    private readonly string _connectionString;
    
    public DapperContext(IConfiguration configuration)
    {
        //Get connection string from appsettings.json
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
                            ?? throw new ArgumentNullException("Connection string not found");
    }
    
  
    //Creates a new database connection and each repository will call this to get a connection
    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}