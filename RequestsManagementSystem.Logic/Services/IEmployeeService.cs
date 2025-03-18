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
        Task<IEnumerable<EmployeeExcelDto>> GetEmployeesToExcelFormat(DateOnly? startDate, DateOnly? EndDate);
        Task<EmployeeDto> GetEmployeeByCodeAsync(string code);
    }
}
