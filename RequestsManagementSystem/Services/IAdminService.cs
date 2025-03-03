using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Dtos.EmployeeDtos;

namespace RequestsManagementSystem.Services
{
    public interface IAdminService
    {
        Task<byte[]> ExportEmployeesToExcel(DateOnly? startDate, DateOnly? EndDate);
        Task<List<EmployeeExcelDto>> ImportEmployeesFromExcel(IFormFile file);
    }
}
