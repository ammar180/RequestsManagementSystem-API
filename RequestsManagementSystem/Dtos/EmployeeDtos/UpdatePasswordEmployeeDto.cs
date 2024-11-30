using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace RequestsManagementSystem.Dtos.EmployeeDtos
{
    public class UpdatePasswordEmployeeDto
    {
        public int EmployeeId { get; set; } = 0;

        [StringLength(200)]
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
