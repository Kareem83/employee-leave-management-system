
namespace ITS_Task.Business.DTOs
{
    public class ManagerDashboardDto
    {
        public int TotalEmployees { get; set; }
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }

        public List<EmployeeDto> TeamMembers { get; set; } = new();
        public List<LeaveRequestDto> LeaveRequests { get; set; } = new();
    }
}
