namespace RequestsManagementSystem.Dtos.EmployeeDtos
{
    public class LoginResultDto : BaseResponse
    {
        public string token { get; set; } = string.Empty;
        public string refreshToken { get; set; } = string.Empty;
        public EmployeeDto ?EmployeeDto { get; set; } = null;
    }
}
