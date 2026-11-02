
using ITS_Task.DataAccess.Interfaces;
using ITS_Task.Models;
using Microsoft.EntityFrameworkCore;

namespace ITS_Task.DataAccess.Repositories
{
    public class LeaveBalanceRepository : ILeaveBalanceRepository
    {
        private readonly AppDbContext _context;

        public LeaveBalanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LeaveBalance>> GetAllAsync()
        {
            return await _context.LeaveBalances.ToListAsync();
        }

        public async Task<LeaveBalance?> GetByIdAsync(int id)
        {
            return await _context.LeaveBalances
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<LeaveBalance?> GetByEmployeeTypeYearAsync(
            int employeeId, int leaveTypeId, int year)
        {
            return await _context.LeaveBalances
                .FirstOrDefaultAsync(b =>
                    b.EmployeeId == employeeId &&
                    b.LeaveTypeId == leaveTypeId &&
                    b.Year == year);
        }

        public async Task<LeaveBalance> AddAsync(LeaveBalance balance)
        {
            await _context.LeaveBalances.AddAsync(balance);
            await _context.SaveChangesAsync();

            return balance;
        }

        public async Task UpdateAsync(LeaveBalance balance)
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<LeaveBalance>> GetByEmployeeAndYearAsync(
    int employeeId, int year)
        {
            return await _context.LeaveBalances
                .Where(b => b.EmployeeId == employeeId && b.Year == year)
                .ToListAsync();
        }

        public async Task<List<LeaveBalance>> GetByEmployeeIdAsync(
    int employeeId)
        {
            return await _context.LeaveBalances
                .Where(b => b.EmployeeId == employeeId)
                .OrderByDescending(b => b.Year)
                .ToListAsync();
        }
    }
}
