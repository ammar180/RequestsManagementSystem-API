using RequestsManagementSystem.Dtos.TransactionsDtos;

namespace RequestsManagementSystem.Services
{
	public interface ITransactionService
	{
        Task<bool> AddTransactionAsync(CreateTransactionDto transactionDto);
        Task<IEnumerable<StaffTransactionDto>> GetStaffTransaction(int managerId);
		Task<IEnumerable<GetTransactionByEmployeeDto>> GetAllTransactionsByEmployeeId(int EmployeeId);
		Task SetSeenStatus(int id, string whoSeen);
	}
}
