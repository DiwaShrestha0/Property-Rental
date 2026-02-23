using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PropertyRental.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PropertyRental
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PropertyAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PropertyAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PropertyAPI
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Property>>> GetProperties()
        {
            return await _context.Properties.ToListAsync();
        }

        // GET: api/PropertyAPI/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Property>> GetProperty(int id)
        {
            var property = await _context.Properties.FindAsync(id);

            if (property == null)
            {
                return NotFound();
            }

            return property;
        }

        // POST: api/PropertyAPI
        [HttpPost]
        public async Task<ActionResult<Property>> PostProperty(Property property)
        {
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProperty), new { id = property.Id }, property);
        }

        // PUT: api/PropertyAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProperty(int id, Property property)
        {
            if (id != property.Id)
            {
                return BadRequest();
            }

            _context.Entry(property).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropertyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(property);
        }

        // DELETE: api/PropertyAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            return Ok(property);
        }

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
        }
    }
}
