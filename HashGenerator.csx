#r "nuget: BCrypt.Net-Next, 4.0.3"

using BCrypt.Net;

string password = "Test@123";
string hash = BCrypt.HashPassword(password, 11);

Console.WriteLine($"Password: {password}");
Console.WriteLine($"Hash: {hash}");
Console.WriteLine();
Console.WriteLine("SQL to insert:");
Console.WriteLine($"DELETE FROM Users WHERE Email = 'test@himalayantech.com.np';");
Console.WriteLine($"INSERT INTO Users (Email, PasswordHash, RoleId, IsActive, CreatedAt, FailedLoginAttempts)");
Console.WriteLine($"VALUES ('test@himalayantech.com.np', '{hash}', 1, 1, GETUTCDATE(), 0);");
