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

        //[Authorize(Roles = "Customer")]
        //public async Task<IActionResult> RetuBike(Rental updatedRental)
        //{
        //    if (updatedRental == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Rentals.Update(updatedRental);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("Index", "Rentals");
        //}
    }
}
