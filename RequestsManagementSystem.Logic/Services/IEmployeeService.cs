using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.DTOs.api.EmployeeDtos;

namespace RequestsManagementSystem.Logic.Services
{
    public interface IEmployeeService
    {
        Task<LoginResultDto> LoginAsync(LoginEmployeeDto loginEmployeeDto);
        Task<EmployeeDto> GetEmployeeDataAsync(int id);
        Task<UpdatePasswordResultDto> UpdatePasswordAsync(UpdatePasswordEmployeeDto EmployeeDto);
        Task<IEnumerable<EmployeeIdAndNameDto>> GetEmployeesAsync(string departmentName);
        (double CasualBalance, double RegularBalance) GetEmployeeBalance(Employee employee, DateOnly? p_startDate = null, DateOnly? p_endDate = null);
        Task<IEnumerable<EmployeeExcelDto>> GetEmployeesToExcelFormat(DateOnly? startDate, DateOnly? EndDate);
        Task<EmployeeDto> GetEmployeeByCodeAsync(string code);
    }
}
