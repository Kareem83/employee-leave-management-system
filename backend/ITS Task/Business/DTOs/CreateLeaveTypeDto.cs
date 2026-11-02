using System.ComponentModel.DataAnnotations;

namespace ITS_Task.Business.DTOs
{
    public class CreateLeaveTypeDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0, 365)]
        public int DefaultDays { get; set; }
    }
}