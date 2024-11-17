using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequestsManagementSystem.Core.Entities
{
	public class Employee
	{
        [Key]
        public int EmployeeId { get; set; }

        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string Password { get; set; } = string.Empty;

        public int VacationsBalance { get; set; }

        public DateOnly DateOfEmployment { get; set; }

        [StringLength(200)]
        public string DepartmentName { get; set; } = string.Empty;

        public int? ManagerId { get; set; } // Allow null for employees without a manager

        // Navigation property for the manager
        [ForeignKey("ManagerId")]
        public Employee? Manager { get; set; }
    }
}
