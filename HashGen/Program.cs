string password = "Test@123";
string hash = BCrypt.Net.BCrypt.HashPassword(password, 11);

Console.WriteLine($"Password: {password}");
Console.WriteLine($"Hash: {hash}");
Console.WriteLine();
Console.WriteLine("SQL to insert:");
Console.WriteLine($"DELETE FROM Users WHERE Email = 'test@himalayantech.com.np';");
Console.WriteLine($"INSERT INTO Users (Email, PasswordHash, RoleId, IsActive, CreatedAt, FailedLoginAttempts)");
Console.WriteLine($"VALUES ('test@himalayantech.com.np', '{hash}', 1, 1, GETUTCDATE(), 0);");
Console.WriteLine();
Console.WriteLine("Testing verification:");
bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
Console.WriteLine($"Does '{password}' match the hash? {isValid}");

// Also test the existing hash
string existingHash = "$2a$11$6BNlYkOBL3ZsXGvVfVJrUe5.F8rJW5vPjKqY9tAP0qJpOWGqx8zPG";
bool existingValid = BCrypt.Net.BCrypt.Verify(password, existingHash);
Console.WriteLine($"Does '{password}' match existing hash? {existingValid}");
