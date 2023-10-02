using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedalHub.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("FK_UserId")]
        public string UserId { get; set; }

        [Required]
        [ForeignKey("FK_BikeId")]
        public int BikeId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
