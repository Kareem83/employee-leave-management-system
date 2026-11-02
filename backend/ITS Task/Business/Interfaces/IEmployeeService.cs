using ITS_Task.Business.DTOs;
using ITS_Task.Models;


namespace ITS_Task.Business.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<EmployeeDto>> GetAllAsync();

        Task<EmployeeDto> AddAsync(CreateEmployeeDto employeeDto);

        Task<EmployeeDto?> GetByIdAsync(int id);

       
        Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeDto employeeDto);

        Task<bool> DeleteAsync(int id);

        Task<List<EmployeeDto>> SearchAsync(string searchTerm);

        Task<EmployeeDto?> GetMyProfileAsync(int userId);

        Task<List<EmployeeDto>> GetMyTeamAsync(int userId);

    }
}
