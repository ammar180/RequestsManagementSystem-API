using RequestsManagementSystem.Core.Entities;

namespace RequestsManagementSystem.Core.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetEmployeeById(int id);
        Task<IEnumerable<Employee>> GetEmployes();
        Task<bool> AddAsync(Employee employee);
        Task<bool> UpdateAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
    }
}
