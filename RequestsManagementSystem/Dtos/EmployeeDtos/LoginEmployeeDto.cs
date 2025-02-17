using RequestsManagementSystem.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RequestsManagementSystem.Dtos.EmployeeDtos
{
    public class LoginEmployeeDto
    {
        [Required(ErrorMessage = "أدخل كود المستخدم")]
        public string EmployeeCode { get; set; } = string.Empty;
        [StringLength(200)]
        [Required(ErrorMessage ="أدخل كلمة المرور")]
        public string Password { get; set; } = string.Empty;
    }
}
