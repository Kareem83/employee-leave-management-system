namespace ITS_Task.Business.DTOs
{
    public class LeaveTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DefaultDays { get; set; }
        public bool IsActive { get; set; }
    }
}