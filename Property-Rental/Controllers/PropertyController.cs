using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace PropertyRental
{
    [Authorize]
    public class PropertyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PropertyController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
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
        public IActionResult Create(Property property, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "properties");
                
                // Ensure directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }

                property.ImagePath = Path.Combine("images", "properties", uniqueFileName).Replace("\\", "/");
            }

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

        [AllowAnonymous]
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