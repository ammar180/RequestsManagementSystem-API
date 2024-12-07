using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Interfaces;
using RequestsManagementSystem.Dtos.TransactionsDtos;

namespace RequestsManagementSystem.Services
{
    public class TransactionService : ITransactionService
    {

        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<bool> AddTransactionAsync(CreateTransactionDto transactionDto)
        {
			try
			{
                var transaction = new Transaction
                {
                    TransactionId = transactionDto.TransactionId,
                    Title = transactionDto.Title,
                    Type = transactionDto.Type,
                    StartDate = transactionDto.StartDate,
                    EndDate = transactionDto.EndDate,
                    SubstituteEmployeeId = transactionDto.SubstituteEmployeeId,
                    Itinerary = string.Join(",", transactionDto.Itinerary),
                    EmployeeId = transactionDto.EmployeeId
                };
                await _transactionRepository.AddTransactionAsync(transaction);
                return true;
            }
			catch (Exception)
			{
				return false;
			}
        }
    }
}
