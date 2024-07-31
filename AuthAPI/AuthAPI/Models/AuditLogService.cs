using AuthAPI.Data;
using Microsoft.AspNetCore.Identity;

namespace AuthAPI.Models
{
    public class AuditLogService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public AuditLogService(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task LogActionAsync(string userId, string action, string entity, string entityId)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = action,
                Entity = entity,
                EntityId = entityId,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
    }
    
}
