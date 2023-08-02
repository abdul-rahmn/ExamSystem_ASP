using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace ExamsSystem.Models
{
    public class EmailIsValid : ValidationAttribute
    {
        ExamsSystemContext context = new ExamsSystemContext();
        public override bool IsValid(object? value)
        {
            int count = context.AspNetUsers.Where(u => u.Email == value.ToString()).Count();
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
    public class UserModel
    {

        [Required]
        [StringLength(20, MinimumLength = 2,ErrorMessage ="must be greater than 1 letter")]
        public string? FirstName { get; set; }
        [Required]
        [StringLength(20,MinimumLength =2, ErrorMessage = "must be greater than 1 letter")]
        public string? LastName { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailIsValid(ErrorMessage ="Email Already Exist")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Password And Confirm Doesn't Matched")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
