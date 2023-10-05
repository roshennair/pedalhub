using PedalHub.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace PedalHub.Models
{
    public class Rental
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "User ID")]
        public string UserId { get; set; } = null!;

        public PedalHubUser User { get; set; } = null!;

        [Required]
        [Display(Name = "Bike ID")]
        public int BikeId { get; set; }

        public Bike Bike { get; set; } = null!;

        [Required]
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Ongoing")]
        public bool IsOngoing { get; set; } = true; // Default value is true

        [Display(Name = "Total Price")]
        public float? TotalPrice { get; set; } // Nullable float

        [Display(Name = "Duration (Days)")]
        public int? Duration { get; set; } // Nullable int
    }
}
