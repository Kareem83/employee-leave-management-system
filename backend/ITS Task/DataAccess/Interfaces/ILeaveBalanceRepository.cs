
using ITS_Task.Models;

namespace ITS_Task.DataAccess.Interfaces
{
    public interface ILeaveBalanceRepository
    {
        Task<List<LeaveBalance>> GetAllAsync();

        Task<LeaveBalance?> GetByIdAsync(int id);

        Task<LeaveBalance?> GetByEmployeeTypeYearAsync(
            int employeeId, int leaveTypeId, int year);

        Task<LeaveBalance> AddAsync(LeaveBalance balance);

        Task UpdateAsync(LeaveBalance balance);
        Task<List<LeaveBalance>> GetByEmployeeAndYearAsync(
    int employeeId, int year);

        Task<List<LeaveBalance>> GetByEmployeeIdAsync(int employeeId);
    }
}
