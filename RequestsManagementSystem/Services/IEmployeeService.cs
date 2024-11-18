using RequestsManagementSystem.Dtos.EmployeeDtos;

namespace RequestsManagementSystem
{
    public interface IEmployeeService
    {
        Task<string> LoginAsync(LoginEmployeeDto loginEmployeeDto);
    }
}
