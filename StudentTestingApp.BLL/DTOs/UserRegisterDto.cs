using StudentTestingApp.Core.Models;

namespace StudentTestingApp.BLL.DTOs
{
    public class UserRegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}
