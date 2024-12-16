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

        public async Task<IEnumerable<StaffTransactionDto>> GetStaffTransaction(int managerId)
        {
            var transaction = await _transactionRepository.GetStaffTransaction(managerId);
            
            var result = await Task.WhenAll(transaction.Select(async t => new StaffTransactionDto
            {
                TransactionId = t.TransactionId,
                Type = t.Type.GetEnumDescription(),
                SendDays = (DateTime.Now - t.CreationDate).Days,
                DueDate = t.StartDate == t.EndDate? 
                ConvertToArabicDate(t.StartDate): //true
                $"من {ConvertToArabicDate(t.StartDate)} الى {ConvertToArabicDate(t.EndDate)}", //false
                Title = t.Title.GetEnumDescription(),
                EmployeeName = t.Employee.Name,
            }));

            return result.ToList();
        }
        private string ConvertToArabicDate(DateTime date)
        {
            CultureInfo arabicCulture = new CultureInfo("ar-EG");

            string arabicDate = date.ToString("d MMMM", arabicCulture);

            return arabicDate;
        }

    }
}
