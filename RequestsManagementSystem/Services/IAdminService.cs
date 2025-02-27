namespace RequestsManagementSystem.Services
{
    public interface IAdminService
    {
        Task<byte[]> ExportEmployeesToExcel(DateOnly? startDate, DateOnly? EndDate);
    }
}
