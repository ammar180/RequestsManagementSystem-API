using System.ComponentModel.DataAnnotations;

namespace RequestsManagementSystem.DTOs.api.EmployeeDtos
{
    public class EmployeeIdAndNameDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        [StringLength(200)]
        public string EmployeeName { get; set; } = string.Empty;
    }
}
