using System.ComponentModel.DataAnnotations;

namespace ExamsSystem.Models
{
    public class RoleModel
    {
        public string? Id { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "must be greater than 1 letter")]
        public string Name { get; set; }
    }
}
