using System.Collections.Generic;

namespace StudentTestingApp.BLL.DTOs
{
    public class TestCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
