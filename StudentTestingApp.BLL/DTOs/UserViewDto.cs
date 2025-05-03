using StudentTestingApp.Core.Models;
using System;

namespace StudentTestingApp.BLL.DTOs
{
    public class UserViewDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}
