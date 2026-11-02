
using System.ComponentModel.DataAnnotations;

namespace ITS_Task.Business.DTOs
{
    public class RejectLeaveRequestDto
    {
        [Required]
        [MinLength(3)]
        public string RejectionReason { get; set; } = string.Empty;
    }
}
