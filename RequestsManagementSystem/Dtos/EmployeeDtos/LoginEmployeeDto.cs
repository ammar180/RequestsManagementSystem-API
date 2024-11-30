using RequestsManagementSystem.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RequestsManagementSystem.Dtos.EmployeeDtos
{
    public class LoginEmployeeDto
    {
        [Required(ErrorMessage = "أدخل كود المستخدم")]
        public int EmployeeId { get; set; }

        [StringLength(200)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$")]
        [Required(ErrorMessage ="أدخل كلمة المرور")]
        public string Password { get; set; } = string.Empty;
    }
}
