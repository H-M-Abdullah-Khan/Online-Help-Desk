
using System.ComponentModel.DataAnnotations;

namespace Online_Help_Desk.Models
{
    public enum RoleEnum
    {
        Admin = 0,
        EndUser = 1,
        FacilityHead = 2,
        Assignee = 3
    }
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        [Required]
        [StringLength(50)]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? PasswordHash { get; set; }

        [Required]
        public RoleEnum Role { get; set; }

        public bool IsActive { get; set; } = false;

        public DateTime DateCreated { get; set; } = DateTime.Now;
    }

}
