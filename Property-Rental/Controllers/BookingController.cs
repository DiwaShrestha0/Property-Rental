using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Models;
using System.Security.Claims;

namespace PropertyRental.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null) return Unauthorized();
            var userId = int.Parse(userIdStr);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Manager");

            IQueryable<Booking> bookingsQuery = _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.User);

            if (!isAdmin)
            {
                bookingsQuery = bookingsQuery.Where(b => b.UserId == userId);
            }

            var bookings = await bookingsQuery.ToListAsync();
            return View(bookings);
        }

        [Authorize]
        public async Task<IActionResult> Create(int propertyId)
        {
            var property = await _context.Properties.FindAsync(propertyId);
            if (property == null) return NotFound();

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null) return Unauthorized();

            var booking = new Booking
            {
                PropertyId = propertyId,
                Property = property,
                UserId = int.Parse(userIdStr),
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date.AddDays(1),
                Status = "Pending"
            };

            return View(booking);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyId,StartDate,EndDate")] Booking booking)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null) return Unauthorized();
            var userId = int.Parse(userIdStr);
            booking.UserId = userId;
            booking.Status = "Pending";

            var property = await _context.Properties.FindAsync(booking.PropertyId);
            if (property == null) return NotFound();

            if (booking.EndDate <= booking.StartDate)
            {
                ModelState.AddModelError("", "End Date must be after the Start Date.");
            }

            var isOverlapping = await _context.Bookings
                .AnyAsync(b => b.PropertyId == booking.PropertyId &&
                               (b.Status == "Pending" || b.Status == "Approved") &&
                               booking.StartDate < b.EndDate && 
                               booking.EndDate > b.StartDate);

            if (isOverlapping)
            {
                ModelState.AddModelError("", "The property is already booked for the selected dates.");
            }

            if (ModelState.IsValid)
            {
                var days = (booking.EndDate - booking.StartDate).Days;
                if (days <= 0) days = 1;
                booking.TotalPrice = property.Price * days;

                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Booking created successfully and is now pending approval!";
                return RedirectToAction(nameof(Index));
            }

            booking.Property = property;
            return View(booking);
        }

        // Approve (Admin/Manager only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Approve(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = "Approved";
            _context.Update(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Reject (Admin/Manager only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Reject(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = "Rejected";
            _context.Update(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // RentalHistory: shows completed bookings
        public async Task<IActionResult> RentalHistory()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var isAdmin = User.IsInRole("Admin");
            var now = DateTime.Now;

            IQueryable<Booking> historyQuery = _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.User)
                .Where(b => b.EndDate < now && b.Status == "Approved");

            if (!isAdmin)
            {
                historyQuery = historyQuery.Where(b => b.UserId == userId);
            }

            var history = await historyQuery.ToListAsync();
            return View(history);
        }
    }
}
