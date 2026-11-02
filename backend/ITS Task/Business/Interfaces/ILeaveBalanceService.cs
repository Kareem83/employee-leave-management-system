
using ITS_Task.Business.DTOs;

namespace ITS_Task.Business.Interfaces
{
    public interface ILeaveBalanceService
    {
        Task<List<LeaveBalanceDto>> GetAllAsync();

        Task<LeaveBalanceDto?> GetByIdAsync(int id);

        Task<LeaveBalanceDto> AddAsync(CreateLeaveBalanceDto dto);

        Task<LeaveBalanceDto?> UpdateAsync(
            int id, UpdateLeaveBalanceDto dto);

        Task<List<LeaveBalanceDto>> GetEmployeeBalancesAsync(
    int employeeId);
    }
}
