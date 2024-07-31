using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            return await _context.StationTasks
                .Include(t => t.BikeStation)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    StationName = t.BikeStation.Name
                })
                .ToListAsync();
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var stationtask = await _context.StationTasks
                .Include(t => t.BikeStation)
                .Where(t => t.Id == id)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    StationName = t.BikeStation.Name
                })
                .FirstOrDefaultAsync();

            if (stationtask == null)
            {
                return NotFound();
            }

            return stationtask;
        }

        // POST: api/Tasks
        [HttpPost]
        public async Task<ActionResult<StationTask>> PostTask(StationTask stationtask)
        {
            _context.StationTasks.Add(stationtask);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = stationtask.Id }, stationtask);
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(int id, StationTask stationtask)
        {
            if (id != stationtask.Id)
            {
                return BadRequest();
            }

            _context.Entry(stationtask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
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

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var stationtask = await _context.StationTasks.FindAsync(id);
            if (stationtask == null)
            {
                return NotFound();
            }

            _context.StationTasks.Remove(stationtask);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(int id)
        {
            return _context.StationTasks.Any(e => e.Id == id);
        }
    }
}


