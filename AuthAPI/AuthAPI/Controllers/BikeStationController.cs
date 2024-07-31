using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BikeStationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BikeStationController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/BikeStation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BikeStation>>> GetBikeStations()
        {
            return await _context.BikeStations.ToListAsync();
        }

        // GET: api/BikeStation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BikeStation>> GetBikeStation(int id)
        {
            var bikeStation = await _context.BikeStations.FindAsync(id);

            if (bikeStation == null)
            {
                return NotFound();
            }

            return bikeStation;
        }

        // POST: api/BikeStation
        [HttpPost]
        public async Task<ActionResult<BikeStation>> PostBikeStation(BikeStation bikeStation)
        {
        
            _context.BikeStations.Add(bikeStation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBikeStation), new { id = bikeStation.BikeStationId }, bikeStation);
        }

        // PUT: api/BikeStation/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBikeStation(int id, BikeStation bikeStation)
        {
            if (id != bikeStation.BikeStationId)
            {
                return BadRequest();
            }

            _context.Entry(bikeStation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BikeStationExists(id))
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


        // DELETE: api/BikeStation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBikeStation(int id)
        {
            var bikeStation = await _context.BikeStations.FindAsync(id);
            if (bikeStation == null)
            {
                return NotFound();
            }

            _context.BikeStations.Remove(bikeStation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BikeStationExists(int id)
        {
            return _context.BikeStations.Any(e => e.BikeStationId == id);
        }


    }
}