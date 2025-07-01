
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
    public class Users
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        public RoleEnum Role { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
