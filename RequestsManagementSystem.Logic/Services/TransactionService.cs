using Microsoft.EntityFrameworkCore;
using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Enums;
using RequestsManagementSystem.Core.Extentions;
using RequestsManagementSystem.Core.Interfaces.IRepositories;
using RequestsManagementSystem.Core.Interfaces.IServices;
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
                    .Where(x => x.Type.Id == (int)ETransactionType.RegularLeave && x.Status == TransactionStatus.Approved);

                if (employeeTransactions.Any(t => t.StartDate.Date == DateTime.Now.Date.AddDays(-1)) &&
                    employeeTransactions.Any(t => t.StartDate.Date == DateTime.Now.Date.AddDays(-2)))
                {
                    throw new InvalidOperationException("لقد تعديت الحد الأقصى لطلب إجازة عارضه لثلاث أيام متتالية، يمكنك طلب اجازة اعتيادية");
                }

                var transaction = new Transaction
                {
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
            var transaction = (await _transactionRepository.GetTransactionById(transactionId)) ?? throw new InvalidOperationException("ليسة موجودة");
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
                    Type = t.Type.Description,
                    Status = t.Status.GetEnumDescription(),
                    DueDate = GetFormattedDueDate(t.StartDate, t.EndDate),
                    SendDate = t.CreationDate.ConvertToArabicDate(true),
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
                (< 1) => "يوم واحد",
                2 => "يومان",
                (>= 3 and <= 10) => string.Join(' ', days.ToString(), "أيام"),
                _ => string.Join(' ', days.ToString(), "يوم"),
            };
        }

        public async Task<IEnumerable<StaffTransactionDto>> GetStaffTransaction(int managerId)
        {
            var transactions = await _transactionRepository.GetStaffTransaction(managerId);

            var result = transactions
                .Where(t => t.Status == TransactionStatus.Pending)
                .Select(t => new StaffTransactionDto
                {
                    TransactionId = t.Id,
                    Status = t.Status.GetEnumDescription(),
                    Type = t.Type.Description,
                    DueDate = GetFormattedDueDate(t.StartDate, t.EndDate, t.Type.EType == ETransactionType.FullDay),
                    SendDate = t.CreationDate.ConvertToArabicDate(),
                    TakenDays = CalculateTakenDays(t),
                    EmployeeName = t.Employee.Name,
                    Seen = t.SeenStatus.HasFlag(TransactionSeenStatus.ManagerSeen),
                });

            return [.. result];
        }

        protected string GetFormattedDueDate(DateTime StartDate, DateTime EndDate, bool isDatetime = false)
        {
            return (StartDate == EndDate) ?
                            StartDate.ConvertToArabicDate(isDatetime) :
                            (StartDate.Month == EndDate.Month) ?
                            $"من {StartDate.ConvertToArabicDate(isDatetime)} إلى {EndDate.Day}" :
                            $"من {StartDate.ConvertToArabicDate(isDatetime)} الى {EndDate.ConvertToArabicDate(isDatetime)}";
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
            if (transaction is null)
                return null;

            return new TransactionDto
            {
                TransactionId = transaction.Id,
                CreationDate = transaction.CreationDate.ConvertToArabicDate() ?? "",
                EndDate = transaction.StartDate == transaction.EndDate ? "" : transaction.EndDate.ConvertToArabicDate(),
                Itinerary = transaction.Itinerary,
                RespondDate = transaction.RespondDate?.ConvertToArabicDate() ?? "",
                RespondMessage = transaction.RespondMessage,
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

        public async Task<ReportTransactionDTO> EmployeeReport(int EmployeeId, string p_type, DateTime? StartDate, DateTime? EndDate)
        {
            if (!Enum.TryParse(p_type, true, out ETransactionType type)) // type: regular / casual
                throw new KeyNotFoundException("Couldn't Determine report type for transaction!");
            var employee = await _employeeRepo.GetEmployeeById(EmployeeId, [nameof(Employee.Transactions), nameof(Employee.EmployeeLevel)]) ?? throw new KeyNotFoundException("Employee Id doesn't Exist");

            StartDate ??= new DateTime(DateTime.Now.Year, 1, 1);
            EndDate ??= DateTime.Now;

            double TotalLeaves, ConsumedLeaves, AdditionalLeaves, RemainingLeaves;
                        
            TotalLeaves = CalculateLeaveInMonthRange(
                employee.EmployeeLevel.GetbyTransactionType(type), employee.DateOfEmployment,
                DateOnly.FromDateTime(StartDate.Value), DateOnly.FromDateTime(EndDate.Value));

            var leavesGoupedSammary = TotalConsumedLeaves(employee.Transactions.AsQueryable(), DateOnly.FromDateTime(StartDate.Value), DateOnly.FromDateTime(EndDate.Value));

            AdditionalLeaves = leavesGoupedSammary.SingleOrDefault(x => (x.type.EType == type || x.type.EParentType == type) && x.type.Sign == 1).totalConsumedDays; // Additional Regular or Casual leaves

            ConsumedLeaves = leavesGoupedSammary.SingleOrDefault(x => (x.type.EType == type || x.type.EParentType == type) && x.type.Sign == -1).totalConsumedDays;

            RemainingLeaves = (TotalLeaves + AdditionalLeaves + ConsumedLeaves);

            var FilteredTransactions = employee.Transactions.
                                                Where(t => t.Type.EType == type).
                                                Where(t => t.StartDate.Date >= StartDate && t.EndDate <= EndDate).
                                                Where(t => t.Status == TransactionStatus.Approved).ToList();

            return new ReportTransactionDTO
            {
                Title = p_type,
                TotalLeaves = TotalLeaves.ToString("0.00"),
                RemainingLeaves = RemainingLeaves.ToString("0.00"),
                UsedLeaves = ConsumedLeaves.ToString("0.00"),
                AdditionalLeaves = AdditionalLeaves.ToString("0.00"),
                Transactions = FilteredTransactions.Select(t => new TransactionForReportDTO
                {
                    StartDate = t.StartDate.ConvertToArabicDate(),
                    EndDate = t.EndDate.ConvertToArabicDate(),
                    Duration = CalculateTakenDays(t),
                }).ToList()
            };
        }
        public (double CasualBalance, double RegularBalance) GetEmployeeBalance(Employee employee, DateOnly? p_startDate = null, DateOnly? p_endDate = null)
        {
            // Set default start and end dates if not provided
            p_startDate ??= new DateOnly(DateTime.Now.Year, 1, 1);
            p_endDate ??= DateOnly.FromDateTime(DateTime.Now.Date);


            var casualBalance = 0.0;
            var regularBalance = 0.0;

            // get current year leaves
            casualBalance += (CalculateLeaveInMonthRange(employee.EmployeeLevel.CasualLeavePerMonth, employee.DateOfEmployment, p_startDate.Value, p_endDate.Value));
            regularBalance += (CalculateLeaveInMonthRange(employee.EmployeeLevel.RegularLeaveperMonth, employee.DateOfEmployment, p_startDate.Value, p_endDate.Value));


            var consumedLeaves = TotalConsumedLeaves(employee.Transactions.AsQueryable(), p_startDate.Value, p_endDate.Value);

            foreach (var (type, totalConsumedDays) in consumedLeaves)
            {
                switch (type.EType)
                {
                    case ETransactionType.CasualLeave:
                        casualBalance += totalConsumedDays; // i.e. current year consumed leave -3
                        break;
                    case ETransactionType.AdditionalCasualLeave:
                        casualBalance += totalConsumedDays; // i.e. previous year balance +3
                        break;
                    case ETransactionType.RegularLeave:
                        regularBalance += totalConsumedDays; // i.e. current year consumed leave -3
                        break;
                    case ETransactionType.HalfDay:
                        regularBalance += totalConsumedDays; // i.e. current year consumed leave -0.5
                        break;
                    case ETransactionType.QuarterDay:
                        regularBalance += totalConsumedDays; // i.e. current year consumed leave -0.25
                        break;
                    case ETransactionType.AdditionalRegularLeave:
                        regularBalance += totalConsumedDays; // i.e. previous year balance +3
                        break;
                    default:
                        break;
                }
            }

            return (casualBalance, regularBalance);
        }
        protected IEnumerable<(TransactionType type, double totalConsumedDays)> TotalConsumedLeaves(IQueryable<Transaction> transactions, DateOnly p_startdate, DateOnly p_endDate)
        {
            // getting sum of consumed leave in given date range
            var result = transactions
                .Where(t => t.Status == TransactionStatus.Approved
                            // && t.Title == TransactionTitle.Leave
                            && DateOnly.FromDateTime(t.StartDate) >= p_startdate
                            && DateOnly.FromDateTime(t.EndDate) <= p_endDate)
                .Select(t => new { type = t.Type, days = t.Type.Unit * Math.Max(1, (t.EndDate - t.StartDate).Days) * t.Type.Sign })
                .GroupBy(t => t.type)
                .Select(g => new { type = g.Key, total = g.Sum(t => t.days) })
                .ToList();

            return result.Select(item => (item.type, item.total));
        }
        public double CalculateLeaveInMonthRange(double leavesPerMonth, DateOnly employementDate, DateOnly p_startdate, DateOnly p_endDate)
        {
            int monthsCount = CalculateMonthCount(employementDate, p_startdate, p_endDate);
            return leavesPerMonth * monthsCount;
        }
        public int CalculateMonthCount(DateOnly employmentDate, DateOnly startDate, DateOnly endDate)
        {
            if (endDate < startDate)
                return 0;
            // Ensure startDate is not before employmentDate
            if (startDate < employmentDate)
                startDate = employmentDate;
            // Calculate the total months between startDate and endDate
            int monthsCount = (endDate.Year - startDate.Year) * 12 + (endDate.Month - startDate.Month);
            return Math.Max(monthsCount, 0); // Ensure non-negative result
        }


    }
}
