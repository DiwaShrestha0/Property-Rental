using Microsoft.AspNetCore.Mvc;

namespace PropertyRental
{
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Property property)
        {
            _context.Properties.Add(property);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var item = _context.Properties.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
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

        public IActionResult Delete(int id)
        {
            var item = _context.Properties.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
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