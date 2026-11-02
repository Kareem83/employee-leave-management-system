
using System.ComponentModel.DataAnnotations;

namespace ITS_Task.Business.DTOs
{
    public class CreateLeaveBalanceDto
    {
        [Range(1, int.MaxValue)]
        public int EmployeeId { get; set; }

        [Range(1, int.MaxValue)]
        public int LeaveTypeId { get; set; }

        [Range(2000, 2100)]
        public int Year { get; set; }
    }
}
