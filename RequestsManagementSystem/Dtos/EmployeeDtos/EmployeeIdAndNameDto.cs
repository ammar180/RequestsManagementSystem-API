using System.ComponentModel.DataAnnotations;

namespace RequestsManagementSystem.Dtos.EmployeeDtos
{
    public class EmployeeIdAndNameDto
    {
        public int EmployeeId { get; set; }

        [StringLength(200)]
        public string EmployeeName { get; set; } = string.Empty;
    }
}
