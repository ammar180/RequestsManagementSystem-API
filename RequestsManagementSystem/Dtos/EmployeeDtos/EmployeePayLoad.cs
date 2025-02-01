using RequestsManagementSystem.Core.Enums;

namespace RequestsManagementSystem.Dtos.EmployeeDtos
{
    public class EmployeePayLoad
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeRole { get; set; } = Roles.Employee.ToString();
    }
}
