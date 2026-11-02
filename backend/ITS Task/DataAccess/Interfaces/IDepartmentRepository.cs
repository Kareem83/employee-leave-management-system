
using ITS_Task.Models;

namespace ITS_Task.DataAccess.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetByIdAsync(int id);
        Task<List<Department>> GetAllAsync();
        Task<Department?> GetByNameAsync(string name);
        Task<Department> AddAsync(Department department);
        Task UpdateAsync(Department department);
        Task<bool> HasEmployeesAsync(int departmentId);
        Task DeleteAsync(Department department);
    }
}
