using System;
using System.Collections.Generic;

namespace ExamsSystem.Models
{
    public partial class Exam
    {
        public Exam()
        {
            ExamStudents = new HashSet<ExamStudent>();
        }

        public int Id { get; set; }
        public int CourseId { get; set; }
        public DateTime Date { get; set; }
        public int RegNo { get; set; }

        public virtual Course? Course { get; set; } = null!;
        public virtual ICollection<ExamStudent> ExamStudents { get; set; }
    }
}
