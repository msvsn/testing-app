using System;
using System.Collections.Generic;

namespace StudentTestingApp.BLL.DTOs
{
    public class TestViewDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        // Change QuestionViewDto to QuestionDto
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>(); 
        public int QuestionCount => Questions.Count;
    }
}
