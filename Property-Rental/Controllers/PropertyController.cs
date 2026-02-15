using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PropertyRental
{
    [Authorize]
    public class PropertyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Properties.ToList());
        }

        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create(Property property)
        {
            _context.Properties.Add(property);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Edit(int id)
        {
            var item = _context.Properties.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Edit(Property property)
        {
            _context.Properties.Update(property);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var item = _context.Properties.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var item = _context.Properties.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var item = _context.Properties.Find(id);
            if (item == null) return NotFound();

            _context.Properties.Remove(item);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}