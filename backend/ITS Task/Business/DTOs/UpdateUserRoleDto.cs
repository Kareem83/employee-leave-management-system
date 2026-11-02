
using System.ComponentModel.DataAnnotations;
using ITS_Task.Models.Enums;

namespace ITS_Task.Business.DTOs
{
    public class UpdateUserRoleDto
    {
        [Required]
        [EnumDataType(typeof(UserRole))]
        public UserRole? Role { get; set; }
    }
}
