
using ITS_Task.Business.DTOs;
using ITS_Task.Business.Interfaces;
using ITS_Task.DataAccess.Interfaces;
using ITS_Task.Models;

namespace ITS_Task.Business.Services
{
    public class LeaveBalanceService : ILeaveBalanceService
    {
        private readonly ILeaveBalanceRepository _balanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly IEmployeeService _employeeService;
        private readonly ILeaveRequestRepository _requestRepository;



        public LeaveBalanceService(
            ILeaveBalanceRepository balanceRepository,
            IEmployeeRepository employeeRepository,
            ILeaveTypeRepository leaveTypeRepository,
            IEmployeeService employeeService,
            ILeaveRequestRepository requestRepository)
        {
            _balanceRepository = balanceRepository;
            _employeeRepository = employeeRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _employeeService = employeeService;
            _requestRepository = requestRepository;
        }


        public async Task<List<LeaveBalanceDto>> GetAllAsync()
        {
            var balances = await _balanceRepository.GetAllAsync();
            var result = new List<LeaveBalanceDto>();

            foreach (var balance in balances)
            {
                result.Add(await MapToDtoAsync(balance));
            }

            return result;
        }

        public async Task<LeaveBalanceDto?> GetByIdAsync(int id)
        {
            var balance = await _balanceRepository.GetByIdAsync(id);

            return balance == null
                ? null
                : await MapToDtoAsync(balance);
        }

        public async Task<LeaveBalanceDto> AddAsync(
            CreateLeaveBalanceDto dto)
        {
            var employee = await _employeeRepository
                .GetByIdAsync(dto.EmployeeId);

            if (employee == null)
                throw new ArgumentException(
                    "Employee not found or inactive.");

            var leaveType = await _leaveTypeRepository
                .GetByIdAsync(dto.LeaveTypeId);

            if (leaveType == null || !leaveType.IsActive)
                throw new ArgumentException(
                    "Leave type not found or inactive.");

            var existing = await _balanceRepository
                .GetByEmployeeTypeYearAsync(
                    dto.EmployeeId, dto.LeaveTypeId, dto.Year);

            if (existing != null)
                throw new InvalidOperationException(
                    "Leave balance already exists.");

            var balance = new LeaveBalance
            {
                EmployeeId = dto.EmployeeId,
                LeaveTypeId = dto.LeaveTypeId,
                Year = dto.Year,
                AllocatedDays = leaveType.DefaultDays,
                UsedDays = 0
            };

            await _balanceRepository.AddAsync(balance);

            return await MapToDtoAsync(balance);
        }

        public async Task<LeaveBalanceDto?> UpdateAsync(
            int id, UpdateLeaveBalanceDto dto)
        {
            var balance = await _balanceRepository.GetByIdAsync(id);

            if (balance == null)
                return null;

            var pendingDays = await _requestRepository
    .GetPendingDaysAsync(
        balance.EmployeeId,
        balance.LeaveTypeId,
        balance.Year);

            if (dto.AllocatedDays < balance.UsedDays + pendingDays)
                throw new ArgumentException(
                    "Allocated days cannot be less than used and pending days.");

            balance.AllocatedDays = dto.AllocatedDays;

            await _balanceRepository.UpdateAsync(balance);

            return await MapToDtoAsync(balance);

        }


        public async Task<List<LeaveBalanceDto>> GetEmployeeBalancesAsync(
            int employeeId)
        {
            var employee = await _employeeRepository
                .GetByIdAsync(employeeId);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found.");

            var balances = await _balanceRepository
                .GetByEmployeeIdAsync(employeeId);

            var result = new List<LeaveBalanceDto>();

            foreach (var balance in balances)
            {
                result.Add(await MapToDtoAsync(balance));
            }
            /*
            foreach (var balance in balances)
            {
                var pendingDays = await _requestRepository
                    .GetPendingDaysAsync(
                        employeeId,
                        balance.LeaveTypeId,
                        balance.Year);

                var remainingDays =
                    balance.AllocatedDays - balance.UsedDays;

                result.Add(new LeaveBalanceDto
                {
                    Id = balance.Id,
                    EmployeeId = balance.EmployeeId,
                    LeaveTypeId = balance.LeaveTypeId,
                    Year = balance.Year,
                    AllocatedDays = balance.AllocatedDays,
                    UsedDays = balance.UsedDays,
                    RemainingDays = remainingDays,
                    PendingDays = pendingDays,
                    AvailableDays = remainingDays - pendingDays
                });
            }
            */

            return result;
        }

        private async Task<LeaveBalanceDto> MapToDtoAsync(
    LeaveBalance balance)
        {
            var pendingDays = await _requestRepository.GetPendingDaysAsync(
                balance.EmployeeId,
                balance.LeaveTypeId,
                balance.Year);

            var remainingDays = balance.AllocatedDays - balance.UsedDays;

            return new LeaveBalanceDto
            {
                Id = balance.Id,
                EmployeeId = balance.EmployeeId,
                LeaveTypeId = balance.LeaveTypeId,
                Year = balance.Year,
                AllocatedDays = balance.AllocatedDays,
                UsedDays = balance.UsedDays,
                RemainingDays = remainingDays,
                PendingDays = pendingDays,
                AvailableDays = remainingDays - pendingDays
            };
        }
        /*
        private static LeaveBalanceDto MapToDto(LeaveBalance balance)
        {
            return new LeaveBalanceDto
            {
                Id = balance.Id,
                EmployeeId = balance.EmployeeId,
                LeaveTypeId = balance.LeaveTypeId,
                Year = balance.Year,
                AllocatedDays = balance.AllocatedDays,
                UsedDays = balance.UsedDays,
                RemainingDays =
                    balance.AllocatedDays - balance.UsedDays
            };
        }
        */


    }
}
