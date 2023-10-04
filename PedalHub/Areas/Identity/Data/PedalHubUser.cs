using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PedalHub.Models;

namespace PedalHub.Areas.Identity.Data;

// Add profile data for application users by adding properties to the PedalHubUser class
public class PedalHubUser : IdentityUser
{
    [PersonalData]
    public string FullName { get; set; } = null!;

    [PersonalData]
    public string Type { get; set; } = null!;

    public List<Reservation> Reservations { get; set; } = new();
}

