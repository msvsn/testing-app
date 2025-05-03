using System;

namespace StudentTestingApp.BLL.DTOs
{
    public class StudentAnswerDto
    {
        public Guid QuestionId { get; set; }
        public Guid SelectedAnswerId { get; set; } 
    }
}
