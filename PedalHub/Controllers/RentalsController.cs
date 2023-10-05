using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedalHub.Data;
using PedalHub.Models;
using System.Security.Claims;

namespace PedalHub.Controllers
{
    public class RentalsController : Controller
    {

        private readonly PedalHubContext _context;

        public RentalsController(PedalHubContext context)
        {
            _context = context;
        }



        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.IsInRole("Administrator"))
            {
                return RedirectToAction("AdminIndex");
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                Console.WriteLine(currentUserId);
                return RedirectToAction("Index", "Home");
            }

            var rentalsList = await _context.Rentals.Where(r => r.UserId == currentUserId).ToListAsync();


            return View(rentalsList);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AdminIndex()
        {
            List<Rental> rentalsList = await _context.Rentals.ToListAsync();
            return View(rentalsList);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            Rental rental = await _context.Rentals
                .Include(r => r.Bike)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental.Bike != null)
            {
                rental.Bike.AvailableUnits += 1;
            }

            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Rentals");
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ReturnBike(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Rental rental = await _context.Rentals.FindAsync(id);

            if (rental == null)
            {
                return BadRequest("Rental with ID " + id + " is not found.");
            }

            return View(rental);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CalculatePayment(Rental updatedRental)
        {
            if (updatedRental == null)
            {
                return NotFound();
            }

            Rental rental = await _context.Rentals.FindAsync(updatedRental.Id);
            Bike bike = await _context.Bike.FindAsync(updatedRental.BikeId);

            if (rental == null || bike == null)
            {
                return BadRequest("Missing rental or bike information in database");
            }

            rental.Duration = updatedRental.Duration;
            rental.TotalPrice = (float?)(bike.DailyRentalPrice * rental.Duration);

            return RedirectToAction("MakePayment", "Rentals", rental);
        }

        public IActionResult MakePayment(Rental rental)
        {
            return View(rental);
        }

        public async Task<IActionResult> Pay(Rental rental)
        {
            if (rental == null)
            {
                return NotFound();
            }

            Rental paidRental = await _context.Rentals
                .Include(r => r.Bike)
                .FirstOrDefaultAsync(r => r.Id == rental.Id);

            if (paidRental == null)
            {
                return BadRequest("This rental does not exist"); 
            }

            paidRental.Duration = rental.Duration;
            paidRental.TotalPrice = rental.TotalPrice;
            paidRental.IsOngoing = false;

            if (paidRental.Bike != null)
            {
                paidRental.Bike.AvailableUnits += 1;
            }

            _context.Rentals.Update(paidRental);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Rentals");
        }
    }
}
