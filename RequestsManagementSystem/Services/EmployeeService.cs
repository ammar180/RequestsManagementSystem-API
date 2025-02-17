using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Enums;
using RequestsManagementSystem.Core.Interfaces;
using RequestsManagementSystem.Dtos.EmployeeDtos;

namespace RequestsManagementSystem.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IJWTService _JWT;

        public EmployeeService(IEmployeeRepository employeeRepository, IJWTService jWT)
        {
            _employeeRepository = employeeRepository;
            _JWT = jWT;
        }

        public async Task<EmployeeDto> GetEmployeeDataAsync(int id)
        {
            // Validate employee credentials
            var employee = await _employeeRepository.GetEmployeeById(id,
                    [nameof(Employee.Manager), nameof(Employee.Transactions), nameof(Employee.EmployeeLevel)]
                    ) ?? throw new NullReferenceException("المستخدم غير موجود");
            //var balanceResult = GetEmployeeBalance(employee, p_startDate: new DateOnly(2025, 1, 1), p_endDate: new DateOnly(2025, 6, 6));
            var balanceResult = GetEmployeeBalance(employee);
            var resut = new EmployeeDto
            {
                EmployeeId = employee.Id,
                EmployeeCode = employee.Code,
                EmployeeName = employee.Name,
                DepartmentName = employee.DepartmentName,
                DateOfEmployment = employee.DateOfEmployment,
                ManagerName= employee.Manager?.Name ?? "",
                RegularLeaveCount = balanceResult.RegularBalance.ToString("0.00"),
                CasualLeaveCount = balanceResult.CasualBalance.ToString("0.00"),
            };
            return resut;
        }

        // Get List of Employees By Department Name
        public async Task<IEnumerable<EmployeeIdAndNameDto>> GetEmployeesAsync(string departmentName)
        {
            var employees = await _employeeRepository.GetEmployesByDepartment(departmentName);
            if (!employees.Any())
            {
                throw new NullReferenceException("ليس يوجد أي موظفين في هذا القسم");
            }
            return employees.Select(x => new EmployeeIdAndNameDto
            {
                EmployeeId = x.Id,
                EmployeeName = x.Name
            });
        }

        public async Task<LoginResultDto> LoginAsync(LoginEmployeeDto loginEmployeeDto)
        {
            // Validate employee credentials
            var employee = await _employeeRepository.GetEmployeeByCode(loginEmployeeDto.EmployeeCode);

            if (employee == null || employee.Password != loginEmployeeDto.Password)
            {
                throw new UnauthorizedAccessException("خطأ في كلمة السر أو كود المستخدم");
            }
            var payload = new EmployeePayLoad
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.Name,
                EmployeeRole = employee.EmployeeRole.ToString(),
            };
            var token = _JWT.GenerateJwtToken(payload);
            var refreshToken = _JWT.GenerateJwtToken(payload, true);
            return new LoginResultDto
            {
                token= token,
                refreshToken = refreshToken,
                EmployeeDto = await GetEmployeeDataAsync(employee.Id),
                Message="تم تسجيل الدخول بنجاح",
                Status=true
            };
        }
        public async Task<UpdatePasswordResultDto> UpdatePasswordAsync(UpdatePasswordEmployeeDto EmployeeDto)
        {
            // Validate employee credentials
            if(EmployeeDto.EmployeeId == 0)
            {
                return new UpdatePasswordResultDto
                {
                    Status = false,
                    message = "ادخل كود المستخدم",
                };
            }
            if (EmployeeDto.OldPassword == null || EmployeeDto.OldPassword == string.Empty)
            {
                return new UpdatePasswordResultDto
                {
                    Status = false,
                    message = "ادخل كلمه المرور الحالية",
                };
            }
            if (EmployeeDto.Password == null || EmployeeDto.Password == string.Empty)
            {
                return new UpdatePasswordResultDto
                {
                    Status = false,
                    message = "ادخل كلمه المرور الجديدة",
                };
            }
            if (EmployeeDto.ConfirmPassword == null || EmployeeDto.ConfirmPassword == string.Empty)
            {
                return new UpdatePasswordResultDto
                {
                    Status = false,
                    message = "ادخل تاكيد كلمه المرور الجديدة",
                };
            }
            var employee = await _employeeRepository.GetEmployeeById(EmployeeDto.EmployeeId);

            if (employee == null)
            {
                return new UpdatePasswordResultDto
                {
                    Status = false,
                    message = "خطأ في كود المستخدم",
                };
            }
            if (employee.Password != EmployeeDto.OldPassword)
            {
                return new UpdatePasswordResultDto
                {
                    Status = false,
                    message = "كلمة المرور القديمة غير صحيحة",
                };
            }
            if(EmployeeDto.Password != EmployeeDto.ConfirmPassword)
            {
                return new UpdatePasswordResultDto
                {
                    Status = false,
                    message = "برجاء مطابقة تاكيد كلمة المرور",
                };
            }
            employee.Password = EmployeeDto.Password;
            bool response = await _employeeRepository.UpdateAsync(employee);
            if(response == true)
            {
                return new UpdatePasswordResultDto
                {
                    Status = response,
                    message = "تم تحديث كلمه المرور بنجاح",
                    EmployeeDto = new EmployeeDto
                    {
                        EmployeeName = employee.Name,
                        EmployeeId = employee.Id,
                        DepartmentName = employee.DepartmentName
                    }
                };
            }
            else
            {
                return new UpdatePasswordResultDto
                {
                    Status = response,
                    message = "حدث خطأ اثناء عمليه تحديث كلمه المرور",
                };
            }
        }
        public (double CasualBalance, double RegularBalance) GetEmployeeBalance(Employee employee, DateOnly? p_startDate = null, DateOnly? p_endDate = null)
        {
            // Set default start and end dates if not provided
            p_startDate ??= new DateOnly(DateTime.Now.Year, 1, 1);
            p_endDate ??= DateOnly.FromDateTime(DateTime.Now.Date);

            var casualBalance = 0.0;
            var regularBalance = 0.0;

            // get current year leaves
            casualBalance += CalculateLeaveInMonthRange(employee.EmployeeLevel.CasualLeavePerMonth, employee.DateOfEmployment, p_startDate.Value, p_endDate.Value);
            regularBalance += CalculateLeaveInMonthRange(employee.EmployeeLevel.RegularLeaveperMonth, employee.DateOfEmployment, p_startDate.Value, p_endDate.Value);

           
           var consumedLeaves = TotalConsumedLeaves(employee.Transactions.AsQueryable(), p_startDate.Value, p_endDate.Value);
            
            foreach (var (type, totalConsumedDays) in consumedLeaves)
            {
                if (!Enum.TryParse(type.Name, false, out ETransactionType typeResuls))
                    typeResuls = ETransactionType.Other;

                switch (typeResuls)
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
        protected int CalculateMonthCount(DateOnly employmentDate, DateOnly startDate, DateOnly endDate)
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

