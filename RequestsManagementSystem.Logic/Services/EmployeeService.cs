using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Enums;
using RequestsManagementSystem.Core.Interfaces;
using RequestsManagementSystem.DTOs.api.EmployeeDtos;

namespace RequestsManagementSystem.Logic.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITransactionService _transactionService;
        private readonly IJWTService _JWT;

        public EmployeeService(IEmployeeRepository employeeRepository, IJWTService jWT, ITransactionService transactionService)
        {
            _employeeRepository = employeeRepository;
            _JWT = jWT;
            _transactionService = transactionService;
        }

        public async Task<EmployeeDto> GetEmployeeDataAsync(int id)
        {
            // Validate employee credentials
            var employee = await _employeeRepository.GetEmployeeById(id,
                    [nameof(Employee.Manager), nameof(Employee.Transactions), nameof(Employee.EmployeeLevel)]
                    ) ?? throw new NullReferenceException("المستخدم غير موجود");
            //var balanceResult = GetEmployeeBalance(employee, p_startDate: new DateOnly(2025, 1, 1), p_endDate: new DateOnly(2025, 6, 6));
            var balanceResult = _transactionService.GetEmployeeBalance(employee);
            var resut = new EmployeeDto
            {
                EmployeeId = employee.Id,
                EmployeeCode = employee.Code,
                EmployeeName = employee.Name,
                DepartmentName = employee.DepartmentName,
                DateOfEmployment = employee.DateOfEmployment,
                ManagerName = employee.Manager?.Name ?? "",
                RegularLeaveCount = balanceResult.RegularBalance.ToString("0.00"),
                CasualLeaveCount = balanceResult.CasualBalance.ToString("0.00"),
            };
            return resut;
        }
        public async Task<EmployeeDto> GetEmployeeByCodeAsync(string code)
        {
            // Validate input
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("Employee code cannot be null or empty.", nameof(code));

            // Retrieve employee by code, including related entities
            var employee = await _employeeRepository.GetEmployeeByCode(code, new[] { "Manager", "Transactions", "EmployeeLevel" })
                ?? throw new NullReferenceException("المستخدم غير موجود");

            // Calculate employee balance
            var balanceResult = GetEmployeeBalance(employee);

            // Map employee data to EmployeeDto
            var result = new EmployeeDto
            {
                EmployeeId = employee.Id,
                EmployeeCode = employee.Code,
                EmployeeName = employee.Name,
                DepartmentName = employee.DepartmentName,
                DateOfEmployment = employee.DateOfEmployment,
                ManagerName = employee.Manager?.Name ?? string.Empty,
                RegularLeaveCount = balanceResult.RegularBalance.ToString("0.00"),
                CasualLeaveCount = balanceResult.CasualBalance.ToString("0.00"),
            };

            return result;
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

        public async Task<IEnumerable<EmployeeExcelDto>> GetEmployeesToExcelFormat(DateOnly? startDate, DateOnly? EndDate)
        {
            return (await _employeeRepository.GetEmployesIncludeTransactionAsync()).Select(x =>
            {
                var balance = _transactionService.GetEmployeeBalance(x, startDate, EndDate);
                return new EmployeeExcelDto
                {
                    Name = x.Name,
                    Code = x.Code,
                    CausalBalance = balance.CasualBalance,
                    RegularBalance = balance.RegularBalance,
                };
            });
        }
    }
}

