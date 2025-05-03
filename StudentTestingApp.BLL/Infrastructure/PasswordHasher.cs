using BCrypt.Net;
using System;

namespace StudentTestingApp.BLL.Infrastructure
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                return false;
            }
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword, false, HashType.SHA384); 
            }
            catch (BCrypt.Net.SaltParseException ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
                return false;
            }
        }
    }
}