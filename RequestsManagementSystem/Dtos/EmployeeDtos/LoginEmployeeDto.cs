using RequestsManagementSystem.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RequestsManagementSystem.Dtos.EmployeeDtos
{
    public class LoginEmployeeDto
    {
        public int EmployeeId { get; set; }

        [StringLength(200)]
        public string Password { get; set; } = string.Empty;
    }
}
