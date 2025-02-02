using Microsoft.EntityFrameworkCore;
using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Enums;
using RequestsManagementSystem.Core.Extentions;
using RequestsManagementSystem.Core.Interfaces;
using RequestsManagementSystem.Dtos;
using RequestsManagementSystem.Dtos.EmployeeDtos;
using RequestsManagementSystem.Dtos.TransactionsDtos;

namespace RequestsManagementSystem.Services
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
                    .Where(x => x.Title == TransactionTitle.Leave && x.Type == TransactionType.RegularLeave && x.Status == TransactionStatus.Approved);
                
                if (employeeTransactions.Any(t => t.StartDate.Date == DateTime.Now.Date.AddDays(-1)) &&
                    employeeTransactions.Any(t => t.StartDate.Date == DateTime.Now.Date.AddDays(-2)))
                {
                    throw new InvalidOperationException("لقد تعديت الحد الأقصى لطلب إجازة عارضة لثلاث أيام متتالية، يمكنك طلب اجازة اعتيادية");
                }

                var transaction = new Transaction
                {
                    Title = Enum.Parse<TransactionTitle>(transactionDto.Title, true),
                    Type = Enum.Parse<TransactionType>(transactionDto.Type, true),
                    StartDate = transactionDto.StartDate,
                    EndDate = transactionDto.EndDate,
                    SubstituteEmployeeId = transactionDto.SubstituteEmployeeId,
                    Itinerary = transactionDto.Itinerary,
                    EmployeeId = transactionDto.EmployeeId,
                };
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
            var transaction = await _transactionRepository.GetTransactionById(transactionId);
            try
            {
                if (!Enum.TryParse(transactionDto.Title, true, out TransactionTitle title))
                    return new BaseResponse { Status = false, Message = "عنوان الطلب غير صالح" };

                if (!Enum.TryParse(transactionDto.Type, true, out TransactionType type))
                    return new BaseResponse { Status = false, Message = "نوع الطلب غير صالح" };

                if (transactionDto.StartDate > transactionDto.EndDate)
                    return new BaseResponse { Status = false, Message = "لا يمكن أن يكون تاريخ البدء بعد تاريخ الانتهاء" };

                if (title == TransactionTitle.Leave && transactionDto.StartDate.Date < DateTime.Today)
                    return new BaseResponse { Status = false, Message = "!برجاء إدخال تاريخ  بدايه الاجازه بشكل صحيح" };


                if (title == TransactionTitle.Leave && (type == TransactionType.CasualLeave || type == TransactionType.RegularLeave))
                {
                    var days = (transactionDto.EndDate - transactionDto.StartDate).Days;


                    if (type == TransactionType.CasualLeave && days > 2)
                    {
                        throw new InvalidOperationException("!برجاء إدخال تاريخ بدايه الاجازه بشكل صحيح, الاجازه العارضه لا تتجاوز يومين");
                    }
                    if (type == TransactionType.RegularLeave && days > 16)
                    {
                        throw new InvalidOperationException("!برجاء إدخال تاريخ بدايه الاجازه بشكل صحيح, الاجازه الاعتياديه لا تتجاوز 16 يوم");
                    }
                }

                transaction.Title = title;
                transaction.Type = type;
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

        public async Task<TransactionDto?> GetTransactionByIdAsync(int id)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(id, [nameof(Transaction.Employee), nameof(Transaction.SubstituteEmployee)]);
            if(transaction is null)
                return null;

            return new TransactionDto
            {
                TransactionId = transaction.TransactionId,
                CreationDate = transaction.CreationDate,
                EndDate = transaction.StartDate == transaction.EndDate ? "" : transaction.EndDate.ConvertToArabicDate(),
                Itinerary = transaction.Itinerary,
                RespondDate = transaction.RespondDate,  
                RespondMessage =  transaction.RespondMessage,    
                SeenStatus = transaction.SeenStatus.GetEnumDescription(),
                StartDate = transaction.StartDate.ConvertToArabicDate(),
                Status = transaction.Status.GetEnumDescription(),
                SubstituteEmployee = new EmployeeIdAndNameDto
                {
                    EmployeeId = transaction.SubstituteEmployee.EmployeeId,
                    EmployeeName = transaction.SubstituteEmployee.Name
                },
                Employee = new EmployeeIdAndNameDto
                {
                    EmployeeName = transaction.Employee.Name,
                    EmployeeId = transaction.Employee.EmployeeId,
                },
                Title = transaction.Title.GetEnumDescription(),
                Type = transaction.Type.GetEnumDescription(),       
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
                throw new InvalidOperationException("رجاء تقديم رسالة لسبب الرفض.");
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
