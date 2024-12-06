using RequestsManagementSystem.Core.Enums;
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
        public Roles EmployeeRole { get; set; }

        [StringLength(200)]
        public string DepartmentName { get; set; } = string.Empty;

        // Navigation properties
        public int? ManagerId { get; set; }
        [ForeignKey("EmployeeId")]

        public Employee? Manager { get; set; }
        public ICollection<Employee> ManagerStaff { get; set; } = default!;
        public ICollection<Transaction> Transactions { get; set; } = default!;
    }
}
