﻿using System.ComponentModel.DataAnnotations;

namespace Online_Help_Desk.Models
{

        public enum RequestStatus
        {
            Pending = 0,
            Assigned = 1,
            InProgress = 2,
            Closed = 3
        }


    public class Request
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        [StringLength(150)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public RequestStatus Status { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int FacilityId { get; set; }
        public Facility? Facility { get; set; }

        public int? AssignedToUserId { get; set; }
        public User? AssignedToUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
