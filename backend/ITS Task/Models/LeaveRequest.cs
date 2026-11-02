
using ITS_Task.Models.Enums;

namespace ITS_Task.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public int LeaveTypeId { get; set; }
        public LeaveType LeaveType { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int NumberOfDays { get; set; }

        public string Reason { get; set; } = string.Empty;

        public LeaveStatus Status { get; set; }
            = LeaveStatus.Pending;

        public int? ReviewedById { get; set; }
        public Employee? ReviewedBy { get; set; }

        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }
    }
}
