using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExamsSystem.Models
{
    public partial class Question
    {
        public Question()
        {
            UserAnswers = new HashSet<UserAnswer>();
        }

        public int Id { get; set; }
        public int? CourseId { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "must be greater than 1 letter")]
        [Display(Name ="Question")]
        public string? Question1 { get; set; } = null!;
        [Required]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "must be greater than 1 letter")]
        public string? Option1 { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "must be greater than 1 letter")]
        public string? Option2 { get; set; }
        public string? Option3 { get; set; }
        public string? Option4 { get; set; }
        [Required]
        [StringLength(400)]
        public string? Answer { get; set; } = null!;

        public virtual Course? Course { get; set; } = null!;
        public virtual ICollection<UserAnswer>? UserAnswers { get; set; }
    }
}
