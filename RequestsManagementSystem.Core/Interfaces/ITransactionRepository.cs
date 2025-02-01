using RequestsManagementSystem.Core.Entities;

namespace RequestsManagementSystem.Core.Interfaces
{
	public interface ITransactionRepository
	{
        Task<Transaction?> GetTransactionById(int transactionId);
        Task<bool> RemoveTransactionAsync(int transactionId);
        Task<bool> AddTransactionAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetStaffTransaction(int managerId);
        Task<IEnumerable<Transaction>> GetTransactionByEmployeeIdAsync(int EmployeeId);
        Task<Transaction?> GetTransactionByIdAsync(int id, string[]? includes = null);
        Task SaveChanges();
	} 
}
