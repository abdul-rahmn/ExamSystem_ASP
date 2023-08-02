using System;
using System.Collections.Generic;

namespace ExamsSystem.Models
{
    public partial class Certificate
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public double Average { get; set; }
        public DateTime Date { get; set; }
    }
}
