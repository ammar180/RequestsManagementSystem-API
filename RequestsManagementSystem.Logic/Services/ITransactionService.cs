using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.DTOs.api;
using RequestsManagementSystem.DTOs.api.TransactionsDtos;

namespace RequestsManagementSystem.Logic.Services
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
        Task<string> UpdateTransactionStatusAsync(int id, UpdateTransactionStatusDto request);
        int CalculateMonthCount(DateOnly employmentDate, DateOnly startDate, DateOnly endDate);
        double CalculateLeaveInMonthRange(double leavesPerMonth, DateOnly employementDate, DateOnly p_startdate, DateOnly p_endDate);
        (double CasualBalance, double RegularBalance) GetEmployeeBalance(Employee employee, DateOnly? p_startDate = null, DateOnly? p_endDate = null);
        Task<ReportTransactionDTO> EmployeeReport(int EmployeeId, string p_type, DateTime? StartDate, DateTime? EndDate);
    }
}
