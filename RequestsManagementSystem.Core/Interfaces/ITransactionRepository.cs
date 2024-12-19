using RequestsManagementSystem.Core.Entities;

namespace RequestsManagementSystem.Core.Interfaces
{
	public interface ITransactionRepository
	{
        Task<bool> AddTransactionAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetStaffTransaction(int managerId);
        Task<IEnumerable<Transaction>> GetTransactionByEmployeeIdAsync(int EmployeeId);
        Task<Transaction?> GetTransactionByIdAsync(int id);
		Task SaveChanges();
	} 
}
