
namespace ITS_Task.Business.DTOs
{
    public class LeaveBalanceDto
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int LeaveTypeId { get; set; }

        public int Year { get; set; }

        public int AllocatedDays { get; set; }

        public int UsedDays { get; set; }

        public int RemainingDays { get; set; }

        public int PendingDays { get; set; }

        public int AvailableDays { get; set; }
    }
}
