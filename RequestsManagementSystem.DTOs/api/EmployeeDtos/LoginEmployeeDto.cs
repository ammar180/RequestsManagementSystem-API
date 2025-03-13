using System.ComponentModel.DataAnnotations;

namespace RequestsManagementSystem.DTOs.api.EmployeeDtos
{
    public class LoginEmployeeDto
    {
        [Required(ErrorMessage = "أدخل كود المستخدم")]
        public string EmployeeCode { get; set; } = string.Empty;
        [StringLength(200)]
        [Required(ErrorMessage = "أدخل كلمة المرور")]
        public string Password { get; set; } = string.Empty;
    }
}
