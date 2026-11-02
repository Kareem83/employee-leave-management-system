
using ITS_Task.DataAccess.Interfaces;
using ITS_Task.Models;

namespace ITS_Task.DataAccess.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLog log)
        {
            await _context.AuditLogs.AddAsync(log);
        }
    }
}
