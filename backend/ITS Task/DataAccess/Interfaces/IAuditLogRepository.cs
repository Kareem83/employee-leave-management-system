
using ITS_Task.Models;

namespace ITS_Task.DataAccess.Interfaces
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog log);
    }
}
