using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Dtos.EmployeeDtos;

namespace RequestsManagementSystem.Logic.Services
{
    public interface IAdminService
    {
        Task<byte[]> ExportEmployeesToExcel(DateOnly? startDate, DateOnly? EndDate);
        Task<List<EmployeeExcelDto>> ImportEmployeesFromExcel(IFormFile file);
    }
}
