
using ITS_Task.DataAccess.Interfaces;
using ITS_Task.Models;
using ITS_Task.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace ITS_Task.DataAccess.Repositories
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly AppDbContext _context;

        public LeaveRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequest> AddAsync(
            LeaveRequest request)
        {
            await _context.LeaveRequests.AddAsync(request);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<bool> HasOverlappingRequestAsync(
            int employeeId,
            DateTime startDate,
            DateTime endDate)
        {
            //عشان تداخل الفترات الزمنيههه
            return await _context.LeaveRequests.AnyAsync(r =>
                r.EmployeeId == employeeId &&
                (r.Status == LeaveStatus.Pending ||
                 r.Status == LeaveStatus.Approved) &&
                r.StartDate <= endDate &&
                r.EndDate >= startDate);
        }

        public async Task<int> GetPendingDaysAsync(
            int employeeId,
            int leaveTypeId,
            int year)
        {
            //بنحسب أيام الطلبات الـPending علشان نحجزها مؤقتًا
            return await _context.LeaveRequests
                .Where(r =>
                    r.EmployeeId == employeeId &&
                    r.LeaveTypeId == leaveTypeId &&
                    r.Status == LeaveStatus.Pending &&
                    r.StartDate.Year == year &&
                    r.EndDate.Year == year)
                .SumAsync(r => (int?)r.NumberOfDays) ?? 0;
        }

        public async Task<LeaveRequest?> GetByIdAsync(int id)
        {
            return await _context.LeaveRequests
                .Include(r => r.Employee)
                .Include(r => r.LeaveType)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<T> ExecuteInTransactionAsync<T>(
            Func<Task<T>> action)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var result = await action();

            await transaction.CommitAsync();

            return result;
        }
        public async Task<List<LeaveRequest>> GetByEmployeeIdAsync(
     int employeeId)
        {
            return await _context.LeaveRequests
                .Include(r => r.LeaveType)
                .Where(r => r.EmployeeId == employeeId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetByManagerIdAsync(
     int managerId)
        {
            return await _context.LeaveRequests
                .Include(r => r.Employee)
                .Include(r => r.LeaveType)
                .Where(r => r.Employee.ManagerId == managerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetAllAsync()
        {
            return await _context.LeaveRequests
                .Include(r => r.Employee)
                .Include(r => r.LeaveType)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}
