using RequestsManagementSystem.Core.Enums;
using RequestsManagementSystem.Dtos;
using RequestsManagementSystem.Dtos.TransactionsDtos;

namespace RequestsManagementSystem.Services
{
	public interface ITransactionService
	{
        Task<(bool Success, string Message)> CancelTransactionAsync(int transactionId);
        Task<BaseResponse> EditTransactionAsync(int transactionId, UpdateTransactionDto transactionDto);

        Task<bool> AddTransactionAsync(CreateTransactionDto transactionDto);
        Task<IEnumerable<StaffTransactionDto>> GetStaffTransaction(int managerId);
		Task<IEnumerable<GetTransactionByEmployeeDto>> GetAllTransactionsByEmployeeId(int EmployeeId);
		Task SetSeenStatus(int id, string whoSeen);
        Task<TransactionDto?> GetTransactionByIdAsync(int id);

    }
}
