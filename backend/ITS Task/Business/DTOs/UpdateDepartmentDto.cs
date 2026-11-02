using System.ComponentModel.DataAnnotations;

namespace ITS_Task.Business.DTOs
{
    public class UpdateDepartmentDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}