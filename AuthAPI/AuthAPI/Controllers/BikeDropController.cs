using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BikeDropController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BikeDropController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("bike-drop-schools")]
        public IActionResult GetBikeDropSchools()
        {
            var events = _context.BikeDropSchools.Select(b => new
            {
                title = b.SchoolName,
                start = b.DropDate
            }).ToList();
            return Ok(events);
        }
        // GET: api/Meetings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Meeting>>> GetMeetings()
        {
            return await _context.Meetings.ToListAsync();
        }

        // GET: api/Meetings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Meeting>> GetMeeting(int id)
        {
            var meeting = await _context.Meetings.FindAsync(id);

            if (meeting == null)
            {
                return NotFound();
            }

            return meeting;
        }

        // POST: api/Meetings
        [HttpPost]
        public async Task<ActionResult<Meeting>> PostMeeting(Meeting meeting)
        {
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMeeting), new { id = meeting.MeetingId }, meeting);
        }

        // PUT: api/Meetings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeeting(int id, Meeting meeting)
        {
            if (id != meeting.MeetingId)
            {
                return BadRequest();
            }

            _context.Entry(meeting).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeetingExists(id))
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

        // DELETE: api/Meetings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeeting(int id)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }

            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MeetingExists(int id)
        {
            return _context.Meetings.Any(e => e.MeetingId == id);
        }
    }
}
