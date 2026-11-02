
namespace ITS_Task.Business.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }

        public int TotalLeaveRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }

        public List<EmployeeDto> Employees { get; set; } = new();
        public List<LeaveRequestDto> LeaveRequests { get; set; } = new();
    }
}
