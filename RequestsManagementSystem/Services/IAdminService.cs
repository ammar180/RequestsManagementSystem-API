using RequestsManagementSystem.Core.Entities;

namespace RequestsManagementSystem.Services
{
    public interface IAdminService
    {
        Task<byte[]> ExportEmployeesToExcel(DateOnly? startDate, DateOnly? EndDate);
        Task<List<Employee>> ImportEmployeesFromExcel(IFormFile file);
    }
}
