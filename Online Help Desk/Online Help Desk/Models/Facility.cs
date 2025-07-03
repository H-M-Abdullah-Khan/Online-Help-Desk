using System.ComponentModel.DataAnnotations;

namespace Online_Help_Desk.Models
{
    public class Facility
    {
        [Key]
        public int FacilityId { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        public string? Location { get; set; }
    }
}

