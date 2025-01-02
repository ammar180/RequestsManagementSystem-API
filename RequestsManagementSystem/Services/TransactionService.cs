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
                if (!Enum.TryParse(transactionDto.Title, true, out TransactionTitle title))
                    throw new InvalidOperationException("Can't Determined the title of the transaction.");
                if (!Enum.TryParse(transactionDto.Type, true, out TransactionType type))
                    throw new InvalidOperationException("Can't Determined the type of the transaction.");

                if (transactionDto.StartDate > transactionDto.EndDate)
                {
                    throw new ArgumentException("Start date cannot be after the end date.");
                }

                var transaction = new Transaction
                {
                    Title = title,
                    Type = type,
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

            var result =
                transactions.Select(t => new GetTransactionByEmployeeDto
                {
                    TransactionId = t.TransactionId,
                    Title = t.Title.GetEnumDescription(),
                    Type = t.Type.GetEnumDescription(),
                    Status = t.Status.GetEnumDescription(),
                    DueDate = GetFormattedDueDate(t.StartDate, t.EndDate),
                    SendDate = t.CreationDate.ConvertToArabicDate(),
                    TakenDays = CalculateTakenDays(t),
                });
            return [.. result];

        }

        private static string CalculateTakenDays(Transaction t)
        {
            // check parrtial leave
            if (t.Title.Equals(TransactionTitle.Leave) && t.Type.Equals(TransactionType.HalfDay) || t.Type.Equals(TransactionType.QuarterDay))
                return t.Type.GetEnumDescription();
            
            var days = (t.EndDate - t.StartDate).Days;

            return days switch
            {
                0 => "يوم واحد",
                1 => "يوم واحد",
                2 => "يومان",
                (>= 3 and <= 10) => string.Join(' ', days.ToString(), "أيام"),
                _ => string.Join(' ', days.ToString(), "يوم"),
            };
        }

        public async Task<IEnumerable<StaffTransactionDto>> GetStaffTransaction(int managerId)
        {
            var transactions = await _transactionRepository.GetStaffTransaction(managerId);

            var result = await Task.WhenAll((IEnumerable<Task<StaffTransactionDto>>)
                transactions.Select(async t => new StaffTransactionDto
                {
                    TransactionId = t.TransactionId,
                    Title = t.Title.GetEnumDescription(),
                    Type = t.Type.GetEnumDescription(),
                    DueDate = GetFormattedDueDate(t.StartDate, t.EndDate),
                    SendDate = t.CreationDate.ConvertToArabicDate(),
                    TakenDays = CalculateTakenDays(t),
                    EmployeeName = t.Employee.Name,
                    Seen = t.SeenStatus.HasFlag(TransactionSeenStatus.ManagerSeen),
                }));

            return [.. result];
        }

        private static string GetFormattedDueDate(DateTime StartDate, DateTime EndDate)
        {
            return (StartDate == EndDate) ?
                            StartDate.ConvertToArabicDate() :
                            (StartDate.Month == EndDate.Month) ?
                            $"من {StartDate.ConvertToArabicDate()} إلى {EndDate.Day}" :
                            $"من {StartDate.ConvertToArabicDate()} الى {EndDate.ConvertToArabicDate()}";
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
