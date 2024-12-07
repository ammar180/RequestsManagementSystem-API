using RequestsManagementSystem.Core.Entities;

namespace RequestsManagementSystem.Core.Interfaces
{
	public interface ITransactionRepository
	{
        Task<bool> AddTransactionAsync(Transaction transaction);
    }
}
