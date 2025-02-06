
namespace RequestsManagementSystem.Dtos.EmployeeDtos
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty; 
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        // remainding casual leaves
        public string CasualLeaveCount { get; set; } = "0.0";
        public string RegularLeaveCount { get; set; } = "0.0";
        public DateOnly DateOfEmployment { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        // remainding regular leaves
    }
}
