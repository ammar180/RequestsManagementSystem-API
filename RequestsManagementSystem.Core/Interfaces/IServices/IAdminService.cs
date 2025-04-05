using Microsoft.AspNetCore.Http;
using RequestsManagementSystem.DTOs.api.EmployeeDtos;
using RequestsManagementSystem.DTOs.ViewModels;

namespace RequestsManagementSystem.Core.Interfaces.IServices
{
    public interface IAdminService
    {
        Task<byte[]> ExportEmployeesToExcel(DateOnly? startDate, DateOnly? EndDate);
        Task ImportEmployeesFromExcel(IFormFile excelFile);
        Task<List<EmployeeExcelDto>> ExtractEmployeesFromExcelAsync(IFormFile file);
        Task<IEnumerable<EmployeeDashboardDto>> GetEmployeesDashboard(DateOnly? startDate, DateOnly? EndDate);

    }
}
