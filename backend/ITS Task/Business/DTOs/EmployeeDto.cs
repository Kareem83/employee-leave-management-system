
namespace ITS_Task.Business.DTOs
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        public string Department { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public DateTime HireDate { get; set; }

        public int? ManagerId { get; set; }

        public bool IsActive { get; set; }
    }
}
