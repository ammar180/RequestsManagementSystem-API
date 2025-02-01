
namespace RequestsManagementSystem.Dtos.EmployeeDtos
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        // remainding casual leaves
        public int CasualLeaveCount { get; set; }
        public DateOnly DateOfEmployment { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        // remainding regular leaves
        public float RegularLeaveCount { get; set; }
    }
}
