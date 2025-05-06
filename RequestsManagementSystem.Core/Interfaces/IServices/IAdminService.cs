using Microsoft.AspNetCore.Http;
using RequestsManagementSystem.DTOs.api.EmployeeDtos;

namespace RequestsManagementSystem.Core.Interfaces.IServices
{
    public interface IAdminService
    {
        Task<byte[]> ExportEmployeesToExcel(DateOnly? startDate, DateOnly? EndDate);
        Task ImportEmployeesFromExcel(IFormFile excelFile,bool isCasualImportAllowed);
        Task<List<EmployeeExcelDto>> ExtractEmployeesFromExcelAsync(IFormFile file);
    }
}
