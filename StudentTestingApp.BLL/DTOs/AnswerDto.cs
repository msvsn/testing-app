using System;

namespace StudentTestingApp.BLL.DTOs
{
    public class AnswerDto
    {
        public Guid Id { get; set; } 
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } 
    }
}
