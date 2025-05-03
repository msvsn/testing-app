using System;

namespace StudentTestingApp.Core.Models
{
    public class TestResult : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid TestId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
    }
}
