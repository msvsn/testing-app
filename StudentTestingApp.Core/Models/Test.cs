using System;
using System.Collections.Generic;

namespace StudentTestingApp.Core.Models
{
    public class Test : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }
}
