using System;
using System.Collections.Generic;

namespace ExamsSystem.Models
{
    public partial class ExamStudent
    {
        public int Id { get; set; }
        public string? UserId { get; set; } = null!;
        public int? ExamId { get; set; }

        public virtual Exam Exam { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;
    }
}
