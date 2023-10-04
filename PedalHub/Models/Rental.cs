using PedalHub.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace PedalHub.Models
{
    public class Rental
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        public PedalHubUser User { get; set; } = null!;

        [Required]
        public int BikeId { get; set; }

        public Bike Bike { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        public bool IsOngoing { get; set; } = true; // Default value is true

        [Display(Name = "Total Price")]
        public float? TotalPrice { get; set; } // Nullable float

        [Display(Name = "Duration (Days)")]
        public int? Duration { get; set; } // Nullable int

    }
}
