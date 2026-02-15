using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyRental.Models.ViewModels;

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
            var model = new DashboardViewModel
            {
                TotalProperties = _context.Properties.Count(),
                TotalUsers = _context.Users.Count(),
                TotalRoles = _context.Roles.Count(),
                TotalMenus = _context.Menus.Count()
            };

            return View(model);
        }
    }
}
