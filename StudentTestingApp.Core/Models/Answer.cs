namespace StudentTestingApp.Core.Models
{
    public class Answer : BaseEntity 
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } 
    }
}
