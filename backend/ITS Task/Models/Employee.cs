
namespace ITS_Task.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public DateTime HireDate { get; set; }

        public bool IsActive { get; set; } = true;

        // User Relationship (Required)
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Department Relationship (Required)
        public int DepartmentId { get; set; }
        public Department DepartmentInfo { get; set; } = null!;

        // Manager Relationship (Optional)
        public int? ManagerId { get; set; }
        public Employee? Manager { get; set; }

        public ICollection<Employee> TeamMembers { get; set; }
            = new List<Employee>();
    }
}
