using System;
using System.Collections.Generic;

namespace ExamsSystem.Models
{
    public partial class Result
    {
        public int Id { get; set; }
        public int CountTrue { get; set; }
        public int CountFalse { get; set; }
        public double Average { get; set; }
        public string UserId { get; set; } = null!;
        public int CourseId { get; set; }

        public virtual Course Course { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;
    }
}
