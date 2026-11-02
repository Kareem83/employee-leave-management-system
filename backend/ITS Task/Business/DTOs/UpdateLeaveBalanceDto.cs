
using System.ComponentModel.DataAnnotations;

namespace ITS_Task.Business.DTOs
{
    public class UpdateLeaveBalanceDto
    {
        [Range(0, 365)]
        public int AllocatedDays { get; set; }
    }
}
