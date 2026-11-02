using ITS_Task.Business.DTOs;
using ITS_Task.Business.Interfaces;
using ITS_Task.DataAccess.Interfaces;
using ITS_Task.Models;
using ITS_Task.Models.Enums;
using Microsoft.AspNetCore.Identity;


namespace ITS_Task.Business.Services
{
    public class EmployeeService : IEmployeeService
    {

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository,
            IDepartmentRepository departmentRepository,
            IPasswordHasher<User> passwordHasher)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _passwordHasher = passwordHasher;
        }



        public async Task<EmployeeDto> AddAsync(CreateEmployeeDto employeeDto)
        {
            // 1. Check Email
            var existingEmail = await _userRepository
                .GetByEmailAsync(employeeDto.Email);

            if (existingEmail != null)
            {
                throw new InvalidOperationException(
                    "Email already exists");
            }

            // 2. Check Username
            var existingUserName = await _userRepository
                .GetByUserNameAsync(employeeDto.UserName);

            if (existingUserName != null)
            {
                throw new InvalidOperationException(
                    "Username already exists");
            }

            // 3. Validate Hire Date
            if (employeeDto.HireDate.Date > DateTime.UtcNow.Date)
            {
                throw new ArgumentException(
                    "Hire date cannot be in the future");
            }

            // 4. Validate Department
            if (employeeDto.DepartmentId <= 0)
            {
                throw new ArgumentException(
                    "Invalid Department Id");
            }

            var department = await _departmentRepository
                .GetByIdAsync(employeeDto.DepartmentId);

            if (department == null)
            {
                throw new ArgumentException(
                    "Department not found");
            }

            // 5. Validate Manager
            if (employeeDto.ManagerId.HasValue)
            {
                var manager = await _employeeRepository
                    .GetByIdAsync(employeeDto.ManagerId.Value);

                if (manager == null ||
                    !manager.User.IsActive ||
                    manager.User.Role != UserRole.Manager)
                {
                    throw new ArgumentException(
                        "Invalid or inactive manager");
                }
            }

            // 6. Create User
            var user = new User
            {
                UserName = employeeDto.UserName,
                Email = employeeDto.Email,
                Role = UserRole.Employee,
                IsActive = true
            };

            // 7. Hash Password
            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                employeeDto.Password
            );

            // 8. Create Employee
            var employee = new Employee
            {
                FullName = employeeDto.FullName,
                JobTitle = employeeDto.JobTitle,
                HireDate = employeeDto.HireDate,
                DepartmentId = employeeDto.DepartmentId,
                ManagerId = employeeDto.ManagerId,
                User = user,
                IsActive = true
            };

            // 9. Save User + Employee
            var result = await _employeeRepository
                .AddAsync(employee);

            // 10. Return DTO
            return new EmployeeDto
            {
                EmployeeId = result.Id,
                UserId = result.UserId,
                FullName = result.FullName,
                UserName = result.User.UserName,
                Email = result.User.Email,
                DepartmentId = result.DepartmentId,
                Department = department.Name,
                JobTitle = result.JobTitle,
                HireDate = result.HireDate,
                ManagerId = result.ManagerId,
                IsActive = result.IsActive
            };
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return false;

            employee.IsActive = false;
            employee.User.IsActive = false;

            await _employeeRepository.DeleteAsync(employee);

            return true;
        }


        public async Task<List<EmployeeDto>> GetAllAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            return employees.Select(e => new EmployeeDto
            {
                EmployeeId = e.Id,
                UserId = e.UserId,
                FullName = e.FullName,
                UserName = e.User.UserName,
                Email = e.User.Email,
                DepartmentId = e.DepartmentId,
                Department = e.DepartmentInfo.Name,
                JobTitle = e.JobTitle,
                HireDate = e.HireDate,
                ManagerId = e.ManagerId,
                IsActive = e.IsActive
            }).ToList();
        }


        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return null;

            return new EmployeeDto
            {
                EmployeeId = employee.Id,
                UserId = employee.UserId,
                FullName = employee.FullName,
                UserName = employee.User.UserName,
                Email = employee.User.Email,
                DepartmentId = employee.DepartmentId,
                Department = employee.DepartmentInfo.Name,
                JobTitle = employee.JobTitle,
                HireDate = employee.HireDate,
                ManagerId = employee.ManagerId,
                IsActive = employee.IsActive
            };
        }



        public async Task<List<EmployeeDto>> SearchAsync(string searchTerm)
        {
            var employees = await _employeeRepository.SearchAsync(searchTerm);

            return employees.Select(e => new EmployeeDto
            {
                EmployeeId = e.Id,
                UserId = e.UserId,
                FullName = e.FullName,
                UserName = e.User.UserName,
                Email = e.User.Email,
                DepartmentId = e.DepartmentId,
                Department = e.DepartmentInfo.Name,
                JobTitle = e.JobTitle,
                HireDate = e.HireDate,
                ManagerId = e.ManagerId,
                IsActive = e.IsActive
            }).ToList();
        }



        public async Task<EmployeeDto?> UpdateAsync(
            int id,
            UpdateEmployeeDto employeeDto)
        {
            // 1. Get Existing Employee
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return null;

            // 2. Check Email Uniqueness
            var existingUser = await _userRepository
                .GetByEmailAsync(employeeDto.Email);

            if (existingUser != null &&
                existingUser.Id != employee.UserId)
            {
                throw new InvalidOperationException(
                    "Email already exists");
            }

            // 3. Validate Hire Date
            if (employeeDto.HireDate.Date > DateTime.UtcNow.Date)
            {
                throw new ArgumentException(
                    "Hire date cannot be in the future");
            }

            // 4. Validate Department
            if (employeeDto.DepartmentId <= 0)
            {
                throw new ArgumentException(
                    "Invalid Department Id");
            }

            var department = await _departmentRepository
                .GetByIdAsync(employeeDto.DepartmentId);

            if (department == null)
            {
                throw new ArgumentException(
                    "Department not found");
            }

            // 5. Validate Manager
            if (employeeDto.ManagerId.HasValue)
            {
                if (employeeDto.ManagerId.Value == employee.Id)
                {
                    throw new ArgumentException(
                        "Employee cannot be their own manager");
                }

                var manager = await _employeeRepository
                    .GetByIdAsync(employeeDto.ManagerId.Value);

                if (manager == null ||
                    !manager.User.IsActive ||
                    manager.User.Role != UserRole.Manager)
                {
                    throw new ArgumentException(
                        "Invalid or inactive manager");
                }
            }

            // 6. Update User
            employee.User.Email = employeeDto.Email;

            // 7. Update Employee
            employee.FullName = employeeDto.FullName;
            employee.DepartmentId = employeeDto.DepartmentId;
            employee.JobTitle = employeeDto.JobTitle;
            employee.HireDate = employeeDto.HireDate;
            employee.ManagerId = employeeDto.ManagerId;

            // 8. Save Changes
            await _employeeRepository.UpdateAsync(employee);

            // 9. Return Updated DTO
            return new EmployeeDto
            {
                EmployeeId = employee.Id,
                UserId = employee.UserId,
                FullName = employee.FullName,
                UserName = employee.User.UserName,
                Email = employee.User.Email,
                DepartmentId = employee.DepartmentId,
                Department = department.Name,
                JobTitle = employee.JobTitle,
                HireDate = employee.HireDate,
                ManagerId = employee.ManagerId,
                IsActive = employee.IsActive
            };
        }

        public async Task<EmployeeDto?> GetMyProfileAsync(int userId)
        {
            var employee = await _employeeRepository
                .GetByUserIdAsync(userId);

            if (employee == null)
                return null;

            return new EmployeeDto
            {
                EmployeeId = employee.Id,
                UserId = employee.UserId,
                FullName = employee.FullName,
                UserName = employee.User.UserName,
                Email = employee.User.Email,
                DepartmentId = employee.DepartmentId,
                Department = employee.DepartmentInfo.Name,
                JobTitle = employee.JobTitle,
                HireDate = employee.HireDate,
                ManagerId = employee.ManagerId,
                IsActive = employee.IsActive
            };
        }

        public async Task<List<EmployeeDto>> GetMyTeamAsync(int userId)
        {
            var manager = await _employeeRepository
                .GetByUserIdAsync(userId);

            if (manager == null)
                throw new KeyNotFoundException("Manager not found.");

            var employees = await _employeeRepository
                .GetTeamMembersAsync(manager.Id);

            return employees.Select(e => new EmployeeDto
            {
                EmployeeId = e.Id,
                UserId = e.UserId,
                FullName = e.FullName,
                UserName = e.User.UserName,
                Email = e.User.Email,
                DepartmentId = e.DepartmentId,
                Department = e.DepartmentInfo.Name,
                JobTitle = e.JobTitle,
                HireDate = e.HireDate,
                ManagerId = e.ManagerId,
                IsActive = e.IsActive
            }).ToList();
        }

    }
}