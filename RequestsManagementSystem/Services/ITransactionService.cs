using RequestsManagementSystem.Dtos.TransactionsDtos;

namespace RequestsManagementSystem.Services
{
	public interface ITransactionService
	{
        Task<bool> AddTransactionAsync(CreateTransactionDto transactionDto);
    }
}
