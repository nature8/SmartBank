using System.ComponentModel.DataAnnotations;

namespace SmartBank.Authentication.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        // Navigation Property
        public ICollection<User>? Users { get; set; }
    }
}