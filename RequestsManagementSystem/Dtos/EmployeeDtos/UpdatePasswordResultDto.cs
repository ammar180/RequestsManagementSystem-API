namespace RequestsManagementSystem.Dtos.EmployeeDtos
{
    public class UpdatePasswordResultDto
    {
        public string message { get; set; } = string.Empty;
        public bool Status { get; set; }
        public EmployeeDto? EmployeeDto { get; set; } = null;
    }
}
