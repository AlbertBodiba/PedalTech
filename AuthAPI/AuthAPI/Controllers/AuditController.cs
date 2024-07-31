using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuditController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAuditLogs")]
        public async Task<IActionResult> GetAuditLogs()
        {
            var auditLogs = await _context.AuditLogs.ToListAsync();
            return Ok(auditLogs);
        }
    }

}
