using System.Collections.Generic;

namespace StudentTestingApp.Core.Models
{
    public class Question
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Text { get; set; } = string.Empty;
        public List<Answer> Answers { get; set; } = new List<Answer>();
        public int CorrectAnswerIndex { get; set; }
    }
}
