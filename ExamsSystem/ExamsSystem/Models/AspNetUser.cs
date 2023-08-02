using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExamsSystem.Models
{    
    public partial class AspNetUser
    {
        public AspNetUser()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaim>();
            AspNetUserLogins = new HashSet<AspNetUserLogin>();
            AspNetUserTokens = new HashSet<AspNetUserToken>();
            ExamStudents = new HashSet<ExamStudent>();
            Results = new HashSet<Result>();
            UserAnswers = new HashSet<UserAnswer>();
            Roles = new HashSet<AspNetRole>();
        }

        public string Id { get; set; } = null!;
        [Required]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "must be greater than 1 letter")]
        public string? FirstName { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "must be greater than 1 letter")]
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        [Phone(ErrorMessage = "Not valid")]
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual ICollection<ExamStudent> ExamStudents { get; set; }
        public virtual ICollection<Result> Results { get; set; }
        public virtual ICollection<UserAnswer> UserAnswers { get; set; }

        public virtual ICollection<AspNetRole> Roles { get; set; }
    }
}
