
namespace ITS_Task.Business.DTOs
{
    public class LeaveRequestDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfDays { get; set; }

        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string LeaveTypeName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;

        public string? RejectionReason { get; set; }
        public DateTime? ReviewedAt { get; set; }

        public int? ReviewedById { get; set; }
        public string? ReviewerName { get; set; }
    }
}
