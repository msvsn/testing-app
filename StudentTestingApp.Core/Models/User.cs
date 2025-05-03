namespace StudentTestingApp.Core.Models
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }

        // List to store IDs of tests assigned to this user
        public List<Guid> AssignedTestIds { get; set; } = new List<Guid>();
    }
}
