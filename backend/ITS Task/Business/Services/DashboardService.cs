
using ITS_Task.Business.DTOs;
using ITS_Task.Business.Interfaces;
using ITS_Task.DataAccess.Interfaces;
using ITS_Task.Models.Enums;

namespace ITS_Task.Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILeaveRequestRepository _requestRepository;
        private readonly ILeaveBalanceRepository _balanceRepository;
        private readonly IEmployeeService _employeeService;
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly IDepartmentRepository _departmentRepository;
        public DashboardService(
            IEmployeeRepository employeeRepository,
            ILeaveRequestRepository requestRepository,
            ILeaveBalanceRepository balanceRepository, IEmployeeService employeeService,
ILeaveRequestService leaveRequestService, IDepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _requestRepository = requestRepository;
            _balanceRepository = balanceRepository;
            _employeeService = employeeService;
            _leaveRequestService = leaveRequestService;
            _departmentRepository = departmentRepository;
        }

        public async Task<EmployeeDashboardDto?> GetEmployeeDashboardAsync(
            int userId)
        {
            var employee = await _employeeRepository
                .GetByUserIdAsync(userId);

            if (employee == null)
                return null;

            var requests = await _requestRepository
                .GetByEmployeeIdAsync(employee.Id);

            var balances = await _balanceRepository
                .GetByEmployeeAndYearAsync(
                    employee.Id, DateTime.UtcNow.Year);

            return new EmployeeDashboardDto
            {
                TotalRequests = requests.Count,

                PendingRequests = requests.Count(
                    r => r.Status == LeaveStatus.Pending),

                ApprovedRequests = requests.Count(
                    r => r.Status == LeaveStatus.Approved),

                RejectedRequests = requests.Count(
                    r => r.Status == LeaveStatus.Rejected),

                LeaveBalances = balances.Select(b =>
                {
                    var pendingDays = requests
                        .Where(r =>
                            r.LeaveTypeId == b.LeaveTypeId &&
                            r.Status == LeaveStatus.Pending &&
                            r.StartDate.Year == b.Year &&
                            r.EndDate.Year == b.Year)
                        //تجمع عدد أيام الطلبات اللي عدّت من الفلترة:
                        .Sum(r => r.NumberOfDays);

                    var remainingDays = b.AllocatedDays - b.UsedDays;

                    return new LeaveBalanceDto
                    {
                        Id = b.Id,
                        EmployeeId = b.EmployeeId,
                        LeaveTypeId = b.LeaveTypeId,
                        Year = b.Year,
                        AllocatedDays = b.AllocatedDays,
                        UsedDays = b.UsedDays,
                        RemainingDays = remainingDays,
                        PendingDays = pendingDays,
                        AvailableDays = remainingDays - pendingDays
                    };
                }).ToList()
            };
        }


        public async Task<ManagerDashboardDto?> GetManagerDashboardAsync(
            int userId)
        {
            // Get manager Employee record
            var manager = await _employeeRepository
                .GetByUserIdAsync(userId);

            if (manager == null)
                return null;

            // Get team members
            var teamMembers = await _employeeService
                .GetMyTeamAsync(userId);

            // Get team leave requests
            var teamRequests = await _leaveRequestService
                .GetTeamRequestsAsync(userId);

            return new ManagerDashboardDto
            {
                TotalEmployees = teamMembers.Count,
                TotalRequests = teamRequests.Count,

                PendingRequests = teamRequests.Count(
                    r => r.Status == "Pending"),

                ApprovedRequests = teamRequests.Count(
                    r => r.Status == "Approved"),

                RejectedRequests = teamRequests.Count(
                    r => r.Status == "Rejected"),

                TeamMembers = teamMembers,
                LeaveRequests = teamRequests
            };
        }


        public async Task<AdminDashboardDto> GetAdminDashboardAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            var departments = await _departmentRepository.GetAllAsync();
            var requests = await _requestRepository.GetAllAsync();

            return new AdminDashboardDto
            {
                TotalEmployees = employees.Count,
                TotalDepartments = departments.Count,

                TotalLeaveRequests = requests.Count,
                PendingRequests = requests.Count(
                    r => r.Status == LeaveStatus.Pending),
                ApprovedRequests = requests.Count(
                    r => r.Status == LeaveStatus.Approved),
                RejectedRequests = requests.Count(
                    r => r.Status == LeaveStatus.Rejected),

                Employees = employees.Select(e => new EmployeeDto
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
                }).ToList(),

                LeaveRequests = requests.Select(r => new LeaveRequestDto
                {
                    Id = r.Id,
                    EmployeeId = r.EmployeeId,
                    EmployeeName = r.Employee?.FullName ?? "",
                    LeaveTypeId = r.LeaveTypeId,
                    LeaveTypeName = r.LeaveType?.Name ?? "",
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    NumberOfDays = r.NumberOfDays,
                    Reason = r.Reason,
                    Status = r.Status.ToString(),
                    CreatedAt = r.CreatedAt,
                    RejectionReason = r.RejectionReason,
                    ReviewedAt = r.ReviewedAt
                }).ToList()
            };
        }


    }
}
