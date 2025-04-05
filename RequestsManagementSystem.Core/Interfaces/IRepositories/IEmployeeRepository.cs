using RequestsManagementSystem.Core.Entities;

namespace RequestsManagementSystem.Core.Interfaces.IRepositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetEmployeeByCode(string code, string[]? includes = null);
        Task<Employee?> GetEmployeeById(int id, string[]? includes = null);
        Task<bool> AddAsync(Employee employee);
        Task<bool> UpdateAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Employee>> GetEmployesByDepartment(string Department);
        Task<IEnumerable<Employee>> GetEmployesIncludeTransactionAsync();
    }
}
