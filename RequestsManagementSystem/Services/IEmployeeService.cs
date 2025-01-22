using RequestsManagementSystem.Dtos.EmployeeDtos;

namespace RequestsManagementSystem
{
    public interface IEmployeeService
    {
        Task<LoginResultDto> LoginAsync(LoginEmployeeDto loginEmployeeDto);
        Task<EmployeeDto> GetEmployeeDataAsync(int id);
        Task<UpdatePasswordResultDto> UpdatePasswordAsync(UpdatePasswordEmployeeDto EmployeeDto);
        Task<IEnumerable<EmployeeByDepartmentNameDto>> GetEmployeesAsync(string departmentName);
    }
}
