using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.DTOs.api.EmployeeDtos;
using Microsoft.AspNetCore.Http;
namespace RequestsManagementSystem.Logic.Services
{
    public interface IAdminService
    {
        Task<byte[]> ExportEmployeesToExcel(DateOnly? startDate, DateOnly? EndDate);
        Task ImportEmployeesFromExcel(IFormFile excelFile);
        Task<List<EmployeeExcelDto>> ExtractEmployeesFromExcelAsync(IFormFile file);
    }
}
