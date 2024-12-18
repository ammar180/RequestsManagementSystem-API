using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Enums;
using RequestsManagementSystem.Core.Extentions;
using RequestsManagementSystem.Core.Interfaces;
using RequestsManagementSystem.Dtos.EmployeeDtos;
using RequestsManagementSystem.Dtos.TransactionsDtos;
using System.Globalization;

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
                    Title = transactionDto.Title,
                    Type = transactionDto.Type,
                    StartDate = transactionDto.StartDate,
                    EndDate = transactionDto.EndDate,
                    SubstituteEmployeeId = transactionDto.SubstituteEmployeeId,
                    Itinerary = transactionDto.Itinerary,
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

        public async Task<IEnumerable<GetTransactionByEmployeeDto>> GetAllTransactionsByEmployeeId(int EmployeeId)
        {
            var transactions = await _transactionRepository.GetTransactionByEmployeeIdAsync(EmployeeId);

            var result = await Task.WhenAll(transactions.Select(async t => new GetTransactionByEmployeeDto
            {
                Type = t.Type.GetEnumDescription(),
                Status = t.Status.GetEnumDescription(),
                ResponseDate = t.Status == TransactionStatus.Pending ? null : t.CreationDate,
                DueDate = t.StartDate == t.EndDate ?
                t.StartDate.ConvertToArabicDate() : //true
                $"من {t.StartDate.ConvertToArabicDate()} الى {t.EndDate.ConvertToArabicDate()}", //false
                Title = t.Title.GetEnumDescription(),
            })); 
            return result.ToList();

        }

        //SendDays = (DateTime.Now - t.CreationDate).Days,
        public async Task<IEnumerable<StaffTransactionDto>> GetStaffTransaction(int managerId)
        {
            var transaction = await _transactionRepository.GetStaffTransaction(managerId);
            
            var result = await Task.WhenAll(transaction.Select(async t => new StaffTransactionDto
            {
                TransactionId = t.TransactionId,
                Type = t.Type.GetEnumDescription(),
                SendDays = (DateTime.Now - t.CreationDate).Days,
                DueDate = t.StartDate == t.EndDate? 
                t.StartDate.ConvertToArabicDate() : //true
                $"من {t.StartDate.ConvertToArabicDate()} الى {t.EndDate.ConvertToArabicDate()}", //false
                Title = t.Title.GetEnumDescription(),
                EmployeeName = t.Employee.Name,
            }));

            return result.ToList();
        }
       

    }
}
