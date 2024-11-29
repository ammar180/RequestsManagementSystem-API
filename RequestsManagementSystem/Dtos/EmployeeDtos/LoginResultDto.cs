namespace RequestsManagementSystem.Dtos.EmployeeDtos
{
    public class LoginResultDto
    {
        public string message { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public bool Status { get; set; }
        public EmployeeDto ?EmployeeDto { get; set; } = null;
    }
}
