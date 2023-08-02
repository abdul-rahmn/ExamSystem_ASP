using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExamsSystem.Models
{
    public partial class UserAnswer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string UserId { get; set; } = null!;
        [Display(Name = "Your Answer")]
        public string UserAnswer1 { get; set; } = null!;
        public bool StateAnswer { get; set; }

        public virtual Question Question { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;
    }
}
