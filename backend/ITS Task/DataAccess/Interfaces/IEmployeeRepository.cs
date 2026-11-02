
using ITS_Task.Models;

namespace ITS_Task.DataAccess.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllAsync();

        Task<Employee?> GetByIdAsync(int id);

        Task<Employee?> GetByEmailAsync(string email);

        Task<List<Employee>> SearchAsync(string searchTerm);

        Task<Employee> AddAsync(Employee employee);

        Task<Employee> UpdateAsync(Employee employee);

        Task DeleteAsync(Employee employee);
        Task<Employee?> GetByUserIdAsync(int userId);

        Task<List<Employee>> GetTeamMembersAsync(int managerId);
    }
}
