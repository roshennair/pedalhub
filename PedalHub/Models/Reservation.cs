﻿using PedalHub.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedalHub.Models
{
    public class Reservation
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
    }
}
