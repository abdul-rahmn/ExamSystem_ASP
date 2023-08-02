using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ExamsSystem.Models
{
    public class CourseIsValid : ValidationAttribute
    {
        ExamsSystemContext context = new ExamsSystemContext();
        public IHttpContextAccessor HttpContextAccessor =new HttpContextAccessor();


        public override bool IsValid(object? value)
        {
            string userId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int count = context.Courses.Where(c => c.CourseName == value.ToString() && c.UserId == userId).Count();

            if (count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    public partial class Course
    {
        public Course()
        {
            Exams = new HashSet<Exam>();
            Questions = new HashSet<Question>();
            Results = new HashSet<Result>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "must be greater than 1 letter")]
        [CourseIsValid(ErrorMessage = "The Course Is Already Created")]
        [Display(Name ="Course Name")]
        public string CourseName { get; set; } = null!;
        public string? UserId { get; set; }
        public virtual AspNetUser? User { get; set; } = null!;
        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Result> Results { get; set; }
    }
}
