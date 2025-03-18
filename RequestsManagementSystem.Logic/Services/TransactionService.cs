using Microsoft.EntityFrameworkCore;
using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Enums;
using RequestsManagementSystem.Core.Extentions;
using RequestsManagementSystem.Core.Interfaces;
using RequestsManagementSystem.DTOs.api;
using RequestsManagementSystem.DTOs.api.EmployeeDtos;
using RequestsManagementSystem.DTOs.api.TransactionsDtos;

namespace RequestsManagementSystem.Logic.Services
{
    public class TransactionService : ITransactionService
    {

        private readonly ITransactionRepository _transactionRepository;
        private readonly IEmployeeRepository _employeeRepo;

        public TransactionService(ITransactionRepository transactionRepository, IEmployeeRepository employeeRepo)
        {
            _transactionRepository = transactionRepository;
            _employeeRepo = employeeRepo;
        }

        public async Task<(bool Success, string Message)> CancelTransactionAsync(int transactionId)
        {
            var transaction = await _transactionRepository.GetTransactionById(transactionId);

            if (transaction.Status == TransactionStatus.Approved || transaction.Status == TransactionStatus.Rejected)
            {
                throw new InvalidOperationException("لا يمكن إلغاء أو حذف الطلب بعد الموافقة عليه أو رفضه");
            }

            var remove = await _transactionRepository.RemoveTransactionAsync(transactionId);

            return remove ? (true, "تم إلغاء الطلب بنجاح.") : (false, "فشل في إلغاء الطلب .");
        }

        public async Task<bool> AddTransactionAsync(CreateTransactionDto transactionDto)
        {
			try
			{
                var employeeTransactions = await _transactionRepository.GetTransactionByEmployeeIdAsync(transactionDto.EmployeeId);

                employeeTransactions = employeeTransactions
                    .Where(x => x.Title == TransactionTitle.Leave && x.Type.Id == (int)ETransactionType.RegularLeave && x.Status == TransactionStatus.Approved);
                
                if (employeeTransactions.Any(t => t.StartDate.Date == DateTime.Now.Date.AddDays(-1)) &&
                    employeeTransactions.Any(t => t.StartDate.Date == DateTime.Now.Date.AddDays(-2)))
                {
                    throw new InvalidOperationException("لقد تعديت الحد الأقصى لطلب إجازة عارضه لثلاث أيام متتالية، يمكنك طلب اجازة اعتيادية");
                }

                var transaction = new Transaction
                {
                    Title = Enum.Parse<TransactionTitle>(transactionDto.Title, true),
                    StartDate = transactionDto.StartDate,
                    EndDate = transactionDto.EndDate,
                    SubstituteEmployeeId = transactionDto.SubstituteEmployeeId,
                    Itinerary = transactionDto.Itinerary,
                    EmployeeId = transactionDto.EmployeeId,
                };
                // validate transaction id
                transaction.Type = _transactionRepository.GetTransactionTypeIdByName(transactionDto.Type) ?? throw new InvalidOperationException("لم نستطيع تحديد نوع الطلب!");
                // Add the transaction to database via repository
                await _transactionRepository.AddTransactionAsync(transaction);

                // Check if the employee has a manager
                var managerId = transaction.Employee?.ManagerId ??
                                (await _employeeRepo.GetEmployeeById(transaction.EmployeeId))?.ManagerId;

                if (managerId == null)
                    throw new InvalidOperationException("تم حفظ الطلب بنجاح لكن لا يوجد مدير لديك لمراجعه الطلب!");

                return true;
            }
			catch (DbUpdateException)
			{
                throw new InvalidOperationException("حدث خطأ أثناء حفظ الطلب، ربما ادخلت موظف غير متاح");
			}
        }

        public async Task<BaseResponse> EditTransactionAsync(int transactionId, UpdateTransactionDto transactionDto)
        {
            var transaction = (await _transactionRepository.GetTransactionById(transactionId))?? throw new InvalidOperationException("ليسة موجودة");
            try
            {
                transaction.StartDate = transactionDto.StartDate;
                transaction.EndDate = transactionDto.EndDate;
                transaction.SubstituteEmployeeId = transactionDto.SubstituteEmployeeId;
                transaction.Itinerary = transactionDto.Itinerary;


                var updated = await _transactionRepository.UpdateTransactionAsync(transaction);
                if (!updated)
                    return new BaseResponse { Status = false, Message = "فشل في تحديث الطلب" };

                return new BaseResponse { Status = true, Message = "تم تحديث الطلب بنجاح" };
            }
            catch (DbUpdateException)
            {
                return new BaseResponse { Status = false, Message = " حدث خطأ أثناء حفظ الطلب في قاعدة البيانات" };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Status = false, Message = "حدث خطأ غير متوقع: " + ex.Message };
            }
        }

        public async Task<IEnumerable<GetTransactionByEmployeeDto>> GetAllTransactionsByEmployeeId(int EmployeeId)
        {
            var transactions = await _transactionRepository.GetTransactionByEmployeeIdAsync(EmployeeId);

            var result =
                transactions.Select(t => new GetTransactionByEmployeeDto
                {
                    TransactionId = t.Id,
                    Title = t.Title.GetEnumDescription(),
                    Type = t.Type.Description,
                    Status = t.Status.GetEnumDescription(),
                    DueDate = GetFormattedDueDate(t.StartDate, t.EndDate),
                    SendDate = t.CreationDate.ConvertToArabicDate(),
                    TakenDays = CalculateTakenDays(t),
                });
            return [.. result];

        }

        protected string CalculateTakenDays(Transaction t)
        {
            if (t.Type.Unit < 1)
                return t.Type.Description;

            // check parrtial leave
            var days = (t.EndDate - t.StartDate).Days;

            return days switch
            {
                (< 0) => t.Type.Description,
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
                    TransactionId = t.Id,
                    Title = t.Title.GetEnumDescription(),
                    Type = t.Type.Description,
                    DueDate = GetFormattedDueDate(t.StartDate, t.EndDate),
                    SendDate = t.CreationDate.ConvertToArabicDate(),
                    TakenDays = CalculateTakenDays(t),
                    EmployeeName = t.Employee.Name,
                    Seen = t.SeenStatus.HasFlag(TransactionSeenStatus.ManagerSeen),
                }));

            return [.. result];
        }

        protected string GetFormattedDueDate(DateTime StartDate, DateTime EndDate)
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

        public async Task<TransactionDto?> GetTransactionByIdAsync(int id)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(id, [nameof(Transaction.Employee), nameof(Transaction.SubstituteEmployee)]);
            if(transaction is null)
                return null;

            return new TransactionDto
            {
                TransactionId = transaction.Id,
                CreationDate = transaction.CreationDate.ConvertToArabicDate() ?? "",
                EndDate = transaction.StartDate == transaction.EndDate ? "" : transaction.EndDate.ConvertToArabicDate(),
                Itinerary = transaction.Itinerary,
                RespondDate = transaction.RespondDate?.ConvertToArabicDate()?? "",
                RespondMessage =  transaction.RespondMessage,    
                SeenStatus = transaction.SeenStatus.GetEnumDescription(),
                StartDate = transaction.StartDate.ConvertToArabicDate(),
                Status = transaction.Status.GetEnumDescription(),
                SubstituteEmployee = new EmployeeIdAndNameDto
                {
                    EmployeeId = transaction.SubstituteEmployee.Id,
                    EmployeeName = transaction.SubstituteEmployee.Name
                },
                Employee = new EmployeeIdAndNameDto
                {
                    EmployeeName = transaction.Employee.Name,
                    EmployeeId = transaction.Employee.Id,
                },
                Title = transaction.Title.GetEnumDescription(),
                Type = transaction.Type.Description,       
                TakenDays = CalculateTakenDays(transaction),
            };
        }
        public async Task<string> UpdateTransactionStatusAsync(int id, UpdateTransactionStatusDto request)
        {
            //from string to enum
            if (!Enum.TryParse(request.Status, true, out TransactionStatus status))
                throw new ArgumentException("Can't Determined the type of the transaction.");

            // If rejected, responseMessage is required
            if (status == TransactionStatus.Rejected && string.IsNullOrWhiteSpace(request.ResponceMessage))
            {
                throw new InvalidOperationException("برجاء تقديم رسالة لسبب الرفض.");
            }

            // Fetch transaction from database
            var transaction = await _transactionRepository.GetTransactionByIdAsync(id);

            if (transaction == null)
            {
                throw new KeyNotFoundException("Transaction not found.");
            }

            // Update transaction status
            transaction.Status = status;
            transaction.RespondMessage = request.ResponceMessage;
            transaction.RespondDate = DateTime.UtcNow;

            await _transactionRepository.UpdateTransactionAsync(transaction);

            return $"تم تسجيل الرد بنجاح حالة الطلب: {status.GetEnumDescription()}";
        }

    }
}
