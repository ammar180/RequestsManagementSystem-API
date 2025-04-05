using RequestsManagementSystem.DTOs.api.EmployeeDtos;

namespace RequestsManagementSystem.Core.Interfaces.IServices
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
