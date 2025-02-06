using RequestsManagementSystem.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace RequestsManagementSystem.Core.Entities
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;

        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string Password { get; set; } = string.Empty;

        public DateOnly DateOfEmployment { get; set; }

        public Roles EmployeeRole { get; set; }

        public int EmployeeLevelId { get; set; }

        public EmployeeLevel EmployeeLevel { get; set; }

        [StringLength(200)]
        public string DepartmentName { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
        public Employee? Manager { get; set; }
        public ICollection<Employee> ManagerStaff { get; set; } = default!;
        public ICollection<Transaction> Transactions { get; set; } = default!;
    }
}
