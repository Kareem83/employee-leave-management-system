
using ITS_Task.Business.DTOs;

namespace ITS_Task.Business.Interfaces
{
    public interface IDashboardService
    {
        Task<EmployeeDashboardDto?> GetEmployeeDashboardAsync(
            int userId);

        Task<ManagerDashboardDto?> GetManagerDashboardAsync(int userId);

        Task<AdminDashboardDto> GetAdminDashboardAsync();
    }
}
