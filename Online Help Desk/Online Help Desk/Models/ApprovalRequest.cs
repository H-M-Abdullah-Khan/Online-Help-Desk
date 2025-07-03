using Online_Help_Desk.Models;
using System.ComponentModel.DataAnnotations;

namespace Online_Help_Desk.Models
{
    public class ApprovalRequest
    {
        [Key]
        public int ApprovalRequestId { get; set; }

        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public RoleEnum RequestedRole { get; set; }

        public bool IsApproved { get; set; } = false;
        public bool IsRejected { get; set; } = false;

        public DateTime RequestedAt { get; set; } = DateTime.Now;
        public DateTime? ActionTakenAt { get; set; }
    }
}
