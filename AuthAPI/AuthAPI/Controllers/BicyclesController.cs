using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BicycleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BicycleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBicycles()
        {
            var listOfBicycles = await _context.Bicycles.ToListAsync();
            return Ok(listOfBicycles);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Bicycle>> GetBicycle(string id)
        {
            var bicycle = await _context.Bicycles.FindAsync(id);

            if (bicycle == null)
            {
                return NotFound();
            }

            return bicycle;
        }

        [HttpPost]
        public async Task<ActionResult<Bicycle>> PostBicycle(Bicycle bicycle)
        {
            _context.Bicycles.Add(bicycle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBicycle", new { id = bicycle.Id }, bicycle);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBicycle(string id, Bicycle bicycle)
        {
            if (id != bicycle.Id)
            {
                return BadRequest();
            }

            _context.Entry(bicycle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BicycleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBicycle(string id)
        {
            var bicycle = await _context.Bicycles.FindAsync(id);
            if (bicycle == null)
            {
                return NotFound();
            }

            _context.Bicycles.Remove(bicycle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("statistics")]
        public IActionResult GetBicycleStatistics()
        {
            var statistics = new
            {
                Models = _context.Bicycles
                    .GroupBy(b => b.Model)
                    .Select(g => new { Model = g.Key, Count = g.Count() })
                    .ToList(),
                Types = _context.Bicycles
                    .GroupBy(b => b.Type)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToList()
            };

            return Ok(statistics);
        }

        private bool BicycleExists(string id)
        {
            return _context.Bicycles.Any(e => e.Id == id);
        }
    }
}




