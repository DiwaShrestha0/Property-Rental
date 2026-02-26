using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyRental.Models.ViewModels;
using System.Security.Claims;

namespace PropertyRental.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Manager");

            var model = new DashboardViewModel
            {
                TotalProperties = _context.Properties.Count(),
                TotalUsers = isAdmin ? _context.Users.Count() : 0,
                TotalRoles = isAdmin ? _context.Roles.Count() : 0,
                TotalMenus = isAdmin ? _context.Menus.Count() : 0
            };

            if (isAdmin)
            {
                // Admin/Manager: see all bookings
                model.TotalBookings = _context.Bookings.Count();
                model.PendingBookings = _context.Bookings.Count(b => b.Status == "Pending");
            }
            else
            {
                // User: see own bookings
                model.TotalBookings = _context.Bookings.Count(b => b.UserId == userId);
                model.PendingBookings = _context.Bookings.Count(b => b.UserId == userId && b.Status == "Pending");
            }

            return View(model);
        }
    }
}
