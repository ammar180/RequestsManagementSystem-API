using System.ComponentModel.DataAnnotations;

namespace RequestsManagementSystem.Dtos.EmployeeDtos
{
    public class EmployeeByDepartmentNameDto
    {
        public int EmployeeId { get; set; }

        [StringLength(200)]
        public string EmployeeName { get; set; } = string.Empty;
        /*
         [
            {
            "employeeId":1000,
            "employeeNams":"Ammar",
            },
            {
            "employeeId":1001,
            "employeeNams":"Ezz",
            }
         ]
         */
    }
}
