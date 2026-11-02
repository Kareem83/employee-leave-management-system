
using ITS_Task.Business.DTOs;
using ITS_Task.Business.Interfaces;
using ITS_Task.DataAccess.Interfaces;
using ITS_Task.Models;

namespace ITS_Task.Business.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<List<DepartmentDto>> GetAllAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();

            return departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name
            }).ToList();
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);

            if (department == null)
                return null;

            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name
            };
        }

        public async Task<DepartmentDto> AddAsync(CreateDepartmentDto dto)
        {
            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Department name is required.");

            var existing = await _departmentRepository.GetByNameAsync(name);

            if (existing != null)
                throw new InvalidOperationException("Department already exists.");

            var department = new Department
            {
                Name = name
            };

            await _departmentRepository.AddAsync(department);

            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name
            };
        }

        public async Task<DepartmentDto?> UpdateAsync(
            int id, UpdateDepartmentDto dto)
        {
            var department = await _departmentRepository.GetByIdAsync(id);

            if (department == null)
                return null;

            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Department name is required.");

            var existing = await _departmentRepository.GetByNameAsync(name);

            if (existing != null && existing.Id != id)
                throw new InvalidOperationException("Department already exists.");

            department.Name = name;

            await _departmentRepository.UpdateAsync(department);

            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name
            };
        }

        public async Task<string> DeleteAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);

            if (department == null)
                return "NotFound";

            var hasEmployees =
                await _departmentRepository.HasEmployeesAsync(id);

            if (hasEmployees)
                return "HasEmployees";

            await _departmentRepository.DeleteAsync(department);

            return "Success";
        }
    }
}
