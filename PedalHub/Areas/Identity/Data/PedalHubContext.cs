using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PedalHub.Areas.Identity.Data;
using PedalHub.Models;

namespace PedalHub.Data;

public class PedalHubContext : IdentityDbContext<PedalHubUser>
{
    public PedalHubContext(DbContextOptions<PedalHubContext> options)
        : base(options)
    {
    }

    public DbSet<Bike> Bike { get; set; }

    public DbSet<Reservation> Reservation { get; set; }

    public DbSet<Rental> Rentals { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<Reservation>()
            .HasOne(r => r.Bike)
            .WithMany(r => r.Reservations)
            .IsRequired(true);

        builder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(r => r.Reservations)
            .IsRequired(true);
    }
}
