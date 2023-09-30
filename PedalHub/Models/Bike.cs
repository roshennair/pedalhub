using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PedalHub.Models
{
    public class Bike
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Bike Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Daily Rental Price (RM)")]
        public decimal DailyRentalPrice { get; set; }

        [Required]
        [Display(Name = "Available Units")]
        public int AvailableUnits { get; set; }

        [Display(Name = "Image")]
        public string ImageURL { get; set; }
        
        public string ImageKey { get; set; }
    }
}
