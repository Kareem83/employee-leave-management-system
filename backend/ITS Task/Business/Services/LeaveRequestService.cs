
using ITS_Task.Business.DTOs;
using ITS_Task.Business.Interfaces;
using ITS_Task.DataAccess.Interfaces;
using ITS_Task.Models;
using ITS_Task.Models.Enums;

namespace ITS_Task.Business.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _requestRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly ILeaveBalanceRepository _balanceRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public LeaveRequestService(
            ILeaveRequestRepository requestRepository,
            IEmployeeRepository employeeRepository,
            ILeaveTypeRepository leaveTypeRepository,
            ILeaveBalanceRepository balanceRepository,
            IAuditLogRepository auditLogRepository)
        {
            _requestRepository = requestRepository;
            _employeeRepository = employeeRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _balanceRepository = balanceRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<LeaveRequestDto> SubmitAsync(
            int userId,
            CreateLeaveRequestDto dto)
        {
            return await _requestRepository.ExecuteInTransactionAsync(
                () => SubmitCoreAsync(userId, dto)
            );
        }

        private async Task<LeaveRequestDto> SubmitCoreAsync(
            int userId,
            CreateLeaveRequestDto dto)
        {
            var employee = await _employeeRepository
                .GetByUserIdAsync(userId);

            if (employee == null)
                throw new ArgumentException(
                    "No active employee linked to this account.");

            var leaveType = await _leaveTypeRepository
                .GetByIdAsync(dto.LeaveTypeId);

            if (leaveType == null || !leaveType.IsActive)
                throw new ArgumentException(
                    "Leave type not found or inactive.");

            var start = dto.StartDate.Date;
            var end = dto.EndDate.Date;

            if (start < DateTime.UtcNow.Date)
                throw new ArgumentException(
                    "Leave cannot start in the past.");

            if (end < start)
                throw new ArgumentException(
                    "End date cannot be before start date.");

            if (start.Year != end.Year)
                throw new ArgumentException(
                    "Leave requests across two years are not supported yet.");

            var reason = dto.Reason?.Trim();

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException(
                    "Reason is required.");

            var days = CountWorkingDays(start, end);

            if (days == 0)
                throw new ArgumentException(
                    "The selected period has no working days.");

            var overlapping = await _requestRepository
                .HasOverlappingRequestAsync(
                    employee.Id, start, end);

            if (overlapping)
                throw new InvalidOperationException(
                    "Leave request overlaps an existing request.");

            var balance = await _balanceRepository
                .GetByEmployeeTypeYearAsync(
                    employee.Id,
                    dto.LeaveTypeId,
                    start.Year);

            if (balance == null)
                throw new ArgumentException(
                    "Leave balance not found for this year.");

            var pendingDays = await _requestRepository
                .GetPendingDaysAsync(
                    employee.Id,
                    dto.LeaveTypeId,
                    start.Year);

            var availableDays =
                balance.AllocatedDays -
                balance.UsedDays -
                pendingDays;

            if (days > availableDays)
                throw new InvalidOperationException(
                    "Insufficient available leave balance.");

            var request = new LeaveRequest
            {
                EmployeeId = employee.Id,
                LeaveTypeId = dto.LeaveTypeId,
                StartDate = start,
                EndDate = end,
                NumberOfDays = days,
                Reason = reason,
                Status = LeaveStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _requestRepository.AddAsync(request);

            request.LeaveType = leaveType;

            return new LeaveRequestDto
            {
                Id = request.Id,
                EmployeeId = request.EmployeeId,
                LeaveTypeId = request.LeaveTypeId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                NumberOfDays = request.NumberOfDays,
                Reason = request.Reason,
                Status = request.Status.ToString(),
                CreatedAt = request.CreatedAt,
                LeaveTypeName = leaveType.Name,
                EmployeeName = employee.FullName
            };
        }

        private static int CountWorkingDays(
            DateTime start,
            DateTime end)
        {
            int days = 0;

            for (var date = start;
                 date <= end;
                 date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Friday &&
                    date.DayOfWeek != DayOfWeek.Saturday)
                {
                    days++;
                }
            }

            return days;
        }

        public async Task<string> ApproveAsync(
            int requestId,
            int currentUserId,
            bool isAdmin)
        {
            return await _requestRepository.ExecuteInTransactionAsync(
                async () =>
                {
                    var request = await _requestRepository
                        .GetByIdAsync(requestId);

                    if (request == null)
                        return "NotFound";

                    if (request.Status != LeaveStatus.Pending)
                        return "AlreadyReviewed";

                    var reviewer = await _employeeRepository
                        .GetByUserIdAsync(currentUserId);

                    if (!isAdmin &&
                        (reviewer == null ||
                         request.Employee.ManagerId != reviewer.Id))
                        return "Forbidden";

                    if (!request.Employee.IsActive)
                        return "InactiveEmployee";

                    var balance = await _balanceRepository
                        .GetByEmployeeTypeYearAsync(
                            request.EmployeeId,
                            request.LeaveTypeId,
                            request.StartDate.Year);

                    if (balance == null)
                        return "BalanceNotFound";

                    var pendingDays = await _requestRepository
                        .GetPendingDaysAsync(
                            request.EmployeeId,
                            request.LeaveTypeId,
                            request.StartDate.Year);

                    if (balance.UsedDays + pendingDays >
                        balance.AllocatedDays)
                        return "InsufficientBalance";

                    if (balance.UsedDays + request.NumberOfDays >
                        balance.AllocatedDays)
                        return "InsufficientBalance";

                    balance.UsedDays += request.NumberOfDays;

                    request.Status = LeaveStatus.Approved;
                    request.ReviewedById = reviewer?.Id;
                    request.ReviewedAt = DateTime.UtcNow;
                    request.RejectionReason = null;

                    await _auditLogRepository.AddAsync(new AuditLog
                    {
                        UserId = currentUserId,
                        Action = "Approve",
                        EntityName = "LeaveRequest",
                        EntityId = request.Id,
                        CreatedAt = DateTime.UtcNow
                    });

                    await _requestRepository.SaveChangesAsync();

                    return "Success";
                });
        }

        public async Task<string> RejectAsync(
            int requestId,
            int currentUserId,
            bool isAdmin,
            string rejectionReason)
        {
            return await _requestRepository.ExecuteInTransactionAsync(
                async () =>
                {
                    if (string.IsNullOrWhiteSpace(rejectionReason))
                        return "ReasonRequired";

                    var request = await _requestRepository
                        .GetByIdAsync(requestId);

                    if (request == null)
                        return "NotFound";

                    if (request.Status != LeaveStatus.Pending)
                        return "AlreadyReviewed";

                    var reviewer = await _employeeRepository
                        .GetByUserIdAsync(currentUserId);

                    if (!isAdmin &&
                        (reviewer == null ||
                         request.Employee.ManagerId != reviewer.Id))
                        return "Forbidden";

                    request.Status = LeaveStatus.Rejected;
                    request.RejectionReason = rejectionReason.Trim();
                    request.ReviewedById = reviewer?.Id;
                    request.ReviewedAt = DateTime.UtcNow;

                    await _auditLogRepository.AddAsync(new AuditLog
                    {
                        UserId = currentUserId,
                        Action = "Reject",
                        EntityName = "LeaveRequest",
                        EntityId = request.Id,
                        CreatedAt = DateTime.UtcNow
                    });

                    await _requestRepository.SaveChangesAsync();

                    return "Success";
                });
        }

        public async Task<List<LeaveRequestDto>> GetMyRequestsAsync(int userId)
        {
            var employee = await _employeeRepository
                .GetByUserIdAsync(userId);

            if (employee == null)
                throw new ArgumentException("Employee not found.");

            var requests = await _requestRepository
                .GetByEmployeeIdAsync(employee.Id);

            var result = new List<LeaveRequestDto>();

            foreach (var request in requests)
            {
                var dto = await MapToDtoAsync(request);
                result.Add(dto);
            }

            return result;
        }

        public async Task<List<LeaveRequestDto>> GetTeamRequestsAsync(int userId)
        {
            var manager = await _employeeRepository
                .GetByUserIdAsync(userId);

            if (manager == null)
                throw new ArgumentException(
                    "Manager employee record not found.");

            var requests = await _requestRepository
                .GetByManagerIdAsync(manager.Id);

            var result = new List<LeaveRequestDto>();

            foreach (var request in requests)
            {
                var dto = await MapToDtoAsync(request);
                result.Add(dto);
            }

            return result;
        }

        public async Task<string> CancelAsync(
            int requestId,
            int userId)
        {
            return await _requestRepository.ExecuteInTransactionAsync(
                async () =>
                {
                    var employee = await _employeeRepository
                        .GetByUserIdAsync(userId);

                    if (employee == null)
                        return "EmployeeNotFound";

                    var request = await _requestRepository
                        .GetByIdAsync(requestId);

                    if (request == null)
                        return "NotFound";

                    if (request.EmployeeId != employee.Id)
                        return "Forbidden";

                    if (request.Status != LeaveStatus.Pending)
                        return "CannotCancel";

                    request.Status = LeaveStatus.Cancelled;

                    await _requestRepository.SaveChangesAsync();

                    return "Success";
                });
        }

        private async Task<LeaveRequestDto> MapToDtoAsync(
            LeaveRequest request)
        {
            string? reviewerName = null;

            if (request.ReviewedById.HasValue)
            {
                var reviewer = await _employeeRepository
                    .GetByIdAsync(request.ReviewedById.Value);

                reviewerName = reviewer?.FullName;
            }

            return new LeaveRequestDto
            {
                Id = request.Id,
                EmployeeId = request.EmployeeId,
                LeaveTypeId = request.LeaveTypeId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                NumberOfDays = request.NumberOfDays,
                Reason = request.Reason,
                Status = request.Status.ToString(),
                CreatedAt = request.CreatedAt,
                LeaveTypeName =
                    request.LeaveType?.Name ?? string.Empty,
                RejectionReason = request.RejectionReason,
                ReviewedAt = request.ReviewedAt,
                EmployeeName =
                    request.Employee?.FullName ?? string.Empty,
                ReviewedById = request.ReviewedById,
                ReviewerName = reviewerName
            };
        }

        public async Task<List<LeaveRequestDto>> GetAllRequestsAsync()
        {
            var requests = await _requestRepository.GetAllAsync();

            var result = new List<LeaveRequestDto>();

            foreach (var request in requests)
            {
                var dto = await MapToDtoAsync(request);
                result.Add(dto);
            }

            return result;
        }
    }
}
