using System;
using System.Collections.Generic;

namespace StudentTestingApp.BLL.DTOs
{
    public class TestSubmissionDto
    {
        public Guid TestId { get; set; }
        public List<StudentAnswerDto> Answers { get; set; } = new List<StudentAnswerDto>();
    }
}
