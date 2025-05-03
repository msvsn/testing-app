using System;

namespace StudentTestingApp.BLL.DTOs
{
    public class TestResultDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public Guid TestId { get; set; }
        public string TestTitle { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public double Percentage => TotalQuestions > 0 ? (double)Score / TotalQuestions * 100 : 0;
    }
}
