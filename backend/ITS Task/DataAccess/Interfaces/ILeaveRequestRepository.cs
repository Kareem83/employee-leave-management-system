
using ITS_Task.Models;

namespace ITS_Task.DataAccess.Interfaces
{
    public interface ILeaveRequestRepository
    {
        Task<LeaveRequest> AddAsync(LeaveRequest request);

        Task<bool> HasOverlappingRequestAsync(
            int employeeId,
            DateTime startDate,
            DateTime endDate);

        Task<int> GetPendingDaysAsync(
            int employeeId,
            int leaveTypeId,
            int year);
    

    Task<LeaveRequest?> GetByIdAsync(int id);

        Task SaveChangesAsync();

        Task<T> ExecuteInTransactionAsync<T>(
            Func<Task<T>> action);

        Task<List<LeaveRequest>> GetByEmployeeIdAsync(int employeeId);

        Task<List<LeaveRequest>> GetByManagerIdAsync(int managerId);

        Task<List<LeaveRequest>> GetAllAsync();
    }
}