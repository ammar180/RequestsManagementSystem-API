using System.ComponentModel.DataAnnotations.Schema;

namespace RequestsManagementSystem.Models.Entities
{
	public class Employee
	{
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int VacationsBalance { get; set; }
        public DateOnly DateOfEmployment { get; set;}
        public string DepartmentName { get; set; }
        [ForeignKey("EmployeeId")]
        public int ManagerId { get; set; }
    }
}
