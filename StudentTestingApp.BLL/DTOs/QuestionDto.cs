using System;
using System.Collections.Generic;

namespace StudentTestingApp.BLL.DTOs
{
    public class QuestionDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Text { get; set; } = string.Empty;
        public List<AnswerDto> Answers { get; set; } = new List<AnswerDto>();
        // Removed CorrectAnswerIndex property
    }
}
