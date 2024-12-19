using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                TransactionId = t.TransactionId,
                Type = t.Type.GetEnumDescription(),
                Status = t.Status.GetEnumDescription(),
                ResponseDate = t.Status == TransactionStatus.Pending ? null : t.CreationDate,
                DueDate = t.StartDate == t.EndDate ?
                t.StartDate.ConvertToArabicDate() : //true
                $"من {t.StartDate.ConvertToArabicDate()} الى {t.EndDate.ConvertToArabicDate()}", //false
                Title = t.Title.GetEnumDescription(),
                Seen = t.SeenStatus.HasFlag(TransactionSeenStatus.EmployeeSeen),
            })); 
            return [.. result];

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
                t.StartDate.ConvertToArabicDate() : //true
                $"من {t.StartDate.ConvertToArabicDate()} الى {t.EndDate.ConvertToArabicDate()}", //false
                Title = t.Title.GetEnumDescription(),
                EmployeeName = t.Employee.Name,
                Seen = t.SeenStatus.HasFlag(TransactionSeenStatus.ManagerSeen),
            }));

            return [.. result];
        }

		public async Task SetSeenStatus(int id, string whoSeen)
		{
            var transaction = await _transactionRepository.GetTransactionByIdAsync(id) ?? throw new NullReferenceException("Transaction Not found");
            if (!Enum.TryParse(whoSeen, true, out Roles whoSeenEnum))
                throw new InvalidOperationException("Can't Determined who Seen the transaction");
            switch (whoSeenEnum)
            {
                case Roles.Employee:
                    transaction.SeenStatus |= TransactionSeenStatus.EmployeeSeen;
                    break;
                case Roles.Manager:
                    transaction.SeenStatus |= TransactionSeenStatus.ManagerSeen;
                    break;
                default:
                    break;
            }
            await _transactionRepository.SaveChanges();
		}
	}
}
