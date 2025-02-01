using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Enums;
using RequestsManagementSystem.Dtos.EmployeeDtos;

namespace RequestsManagementSystem
{
    public interface IEmployeeService
    {
        Task<LoginResultDto> LoginAsync(LoginEmployeeDto loginEmployeeDto);
        Task<EmployeeDto> GetEmployeeDataAsync(int id);
        Task<UpdatePasswordResultDto> UpdatePasswordAsync(UpdatePasswordEmployeeDto EmployeeDto);
        Task<IEnumerable<EmployeeIdAndNameDto>> GetEmployeesAsync(string departmentName);
        public double GetEmployeeBalance(Employee employee, TransactionType t_type = TransactionType.RegularLeave, DateOnly? p_startDate = null, DateOnly? p_endDate = null);
    }
}
