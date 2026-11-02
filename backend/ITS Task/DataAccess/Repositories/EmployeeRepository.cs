
using ITS_Task.DataAccess.Interfaces;
using ITS_Task.Models;
using Microsoft.EntityFrameworkCore;

namespace ITS_Task.DataAccess.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.User)
                .Include(e => e.DepartmentInfo)
                .Where(e => e.IsActive)
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.User)
                .Include(e => e.DepartmentInfo)
                .FirstOrDefaultAsync(e =>
                    e.Id == id && e.IsActive);
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e =>
                    e.User.Email == email);
        }

        public async Task<List<Employee>> SearchAsync(string searchTerm)
        {
            return await _context.Employees
                .Include(e => e.User)
                .Include(e => e.DepartmentInfo)
                .Where(e => e.IsActive &&
                    (e.FullName.Contains(searchTerm) ||
                     e.User.Email.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);

            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);

            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task DeleteAsync(Employee employee)
        {

            await _context.SaveChangesAsync();
        }

        public async Task<Employee?> GetByUserIdAsync(int userId)
        {
            return await _context.Employees
                .Include(e => e.User)
                .Include(e => e.DepartmentInfo)
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId && e.IsActive);
        }

        public async Task<List<Employee>> GetTeamMembersAsync(int managerId)
        {
            return await _context.Employees
                .Include(e => e.User)
                .Include(e => e.DepartmentInfo)
                .Where(e => e.ManagerId == managerId && e.IsActive)
                .ToListAsync();
        }
    }
}
