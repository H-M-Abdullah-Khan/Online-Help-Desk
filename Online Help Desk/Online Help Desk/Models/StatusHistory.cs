using Online_Help_Desk.Models;
using System.ComponentModel.DataAnnotations;

namespace Online_Help_Desk.Models
{
    public class StatusHistory
    {
        [Key]
        public int StatusHistoryId { get; set; }

        [Required]
        public int RequestId { get; set; }
        public Request Request { get; set; }

        [Required]
        public int UpdatedByUserId { get; set; }
        public User UpdatedByUser { get; set; }

        [Required]
        public RequestStatus Status { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

