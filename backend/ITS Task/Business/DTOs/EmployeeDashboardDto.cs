
namespace ITS_Task.Business.DTOs
{
    public class EmployeeDashboardDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }

        public List<LeaveBalanceDto> LeaveBalances { get; set; }
            = new();
    }
}
