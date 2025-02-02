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
            var balanceResult = GetEmployeeBalance(employee);
            var resut = new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
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
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Name
            });
        }

        public async Task<LoginResultDto> LoginAsync(LoginEmployeeDto loginEmployeeDto)
        {
            // Validate employee credentials

            var employee = await _employeeRepository.GetEmployeeById(loginEmployeeDto.EmployeeId);

            if (employee == null || employee.Password != loginEmployeeDto.Password)
            {
                throw new UnauthorizedAccessException("خطأ في كلمة السر أو كود المستخدم");
            }
            var payload = new EmployeePayLoad
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.Name,
                EmployeeRole = employee.EmployeeRole.ToString(),
            };
            var token = _JWT.GenerateJwtToken(payload);
            var refreshToken = _JWT.GenerateJwtToken(payload, true);
            employee.Manager = await _employeeRepository.GetEmployeeById(employee.ManagerId ?? 0);
            return new LoginResultDto
            {
                token= token,
                refreshToken = refreshToken,
                EmployeeDto = await GetEmployeeDataAsync(loginEmployeeDto.EmployeeId),
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
                        EmployeeId = employee.EmployeeId,
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

            double totalCasualLeaves = CalculateLeaveInMonthRange(employee.EmployeeLevel.CasualLeavePerMonth, employee.DateOfEmployment, (DateOnly)p_startDate, (DateOnly)p_endDate, employee.AdditonalCasualLeaveCount);
            double totalRegularLeaves = CalculateLeaveInMonthRange(employee.EmployeeLevel.RegularLeaveperMonth, employee.DateOfEmployment, (DateOnly)p_startDate, (DateOnly)p_endDate, employee.AdditonalRegularLeaveCount);
            var consumedLeaves = TotalConsumedLeaves(employee.Transactions.AsQueryable(), (DateOnly)p_startDate, p_endDate);

            var casualBalance = totalCasualLeaves - consumedLeaves.FirstOrDefault(c => c.type == TransactionType.CasualLeave).totalConsumed;
            var regularBalance = totalRegularLeaves - consumedLeaves.Where(c => c.type == TransactionType.RegularLeave || c.type == TransactionType.HalfDay || c.type == TransactionType.QuarterDay).Sum(x => x.totalConsumed);

            return (casualBalance, regularBalance);
        }

        private static IEnumerable<(TransactionType type, double totalConsumed)> TotalConsumedLeaves(IQueryable<Transaction> transactions, DateOnly p_startdate, DateOnly? p_endDate = null)
        {
            var result = transactions
                .Where(t => t.Status == TransactionStatus.Approved
                            && t.Title == TransactionTitle.Leave
                            && DateOnly.FromDateTime(t.StartDate) >= p_startdate
                            && (!p_endDate.HasValue || DateOnly.FromDateTime(t.EndDate) <= p_endDate.Value))
                .Select(t => new { type = t.Type, unit = CalculateLeaveDays(t) })
                .GroupBy(t => t.type)
                .Select(g => new { type = g.Key, total = g.Sum(t => t.unit) })
                .ToList();

            return result.Select(item => (item.type, item.total));
        }
        public static double CalculateLeaveInMonthRange(double leavesPerMonth, DateOnly employementDate, DateOnly p_startdate, DateOnly p_endDate, double balance = 0)
        {
            int monthsCount = CalculateMonthCount(employementDate, p_startdate, p_endDate);
            return (leavesPerMonth * monthsCount) + balance;
        }
        private static int CalculateMonthCount(DateOnly employmentDate, DateOnly startDate, DateOnly endDate)
        {
            // Ensure startDate is not before employmentDate
            if (startDate < employmentDate)
                startDate = employmentDate;

            // Calculate the total months between startDate and endDate
            int monthsCount = (endDate.Year - startDate.Year) * 12 + (endDate.Month - startDate.Month);

            // Adjust for partial months
            if (endDate.Day < startDate.Day)
                monthsCount--;

            return Math.Max(monthsCount, 0); // Ensure non-negative result
        }
        private static double CalculateLeaveDays(Transaction t)
        {
            double days = 0;

            if (t.Type == TransactionType.HalfDay)
                days += 0.5;
            else if (t.Type == TransactionType.QuarterDay)
                days += 0.25;
            else
                days = (t.EndDate - t.StartDate).Days;

            return days;
        }
    }
}

