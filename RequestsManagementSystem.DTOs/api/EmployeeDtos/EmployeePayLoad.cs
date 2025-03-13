namespace RequestsManagementSystem.DTOs.api.EmployeeDtos
{
    public class EmployeePayLoad
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeRole { get; set; } = "Employee";
    }
}
