using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedalHub.Areas.Identity.Data;
using PedalHub.Data;
using PedalHub.Models;
using System.Security.Claims;

namespace PedalHub.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly PedalHubContext _context;

        public ReservationsController(PedalHubContext context)
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
            if (currentUserId != null)
            {
                Console.WriteLine(currentUserId);
            }

            List<Reservation> reservationList = await _context.Reservation.Where(r => r.UserId == currentUserId).ToListAsync();
            
            return View(reservationList);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AdminIndex()
        {
            List<Reservation> reservationList = await _context.Reservation.ToListAsync();
            return View(reservationList);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Reservation reservation = await _context.Reservation.FindAsync(id);

            if (reservation == null)
            {
                return BadRequest("Reservation with ID " + id + " was not found.");
            }

            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Reservations");
        }
    }
}
