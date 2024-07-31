using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BikeDropEventController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BikeDropEventController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/BikeDropEvent
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BikeDropEvent>>> GetBikeDropEvents()
        {
            return await _context.BikeDropEvents.ToListAsync();
        }

        // GET: api/BikeDropEvent/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BikeDropEvent>> GetBikeDropEvent(int id)
        {
            var bikeDropEvent = await _context.BikeDropEvents.FindAsync(id);

            if (bikeDropEvent == null)
            {
                return NotFound();
            }

            return bikeDropEvent;
        }

        // POST: api/BikeDropEvent
        [HttpPost]
        public async Task<ActionResult<BikeDropEvent>> PostBikeDropEvent(BikeDropEvent bikeDropEvent)
        {
            _context.BikeDropEvents.Add(bikeDropEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBikeDropEvent), new { id = bikeDropEvent.BikeDropEventId }, bikeDropEvent);
        }

        // PUT: api/BikeDropEvent/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBikeDropEvent(int id, BikeDropEvent bikeDropEvent)
        {
            if (id != bikeDropEvent.BikeDropEventId)
            {
                return BadRequest();
            }

            _context.Entry(bikeDropEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BikeDropEventExists(id))
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

        // DELETE: api/BikeDropEvent/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBikeDropEvent(int id)
        {
            var bikeDropEvent = await _context.BikeDropEvents.FindAsync(id);
            if (bikeDropEvent == null)
            {
                return NotFound();
            }

            _context.BikeDropEvents.Remove(bikeDropEvent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BikeDropEventExists(int id)
        {
            return _context.BikeDropEvents.Any(e => e.BikeDropEventId == id);
        }
    }
}
