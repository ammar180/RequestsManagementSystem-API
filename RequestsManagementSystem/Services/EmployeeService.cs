using RequestsManagementSystem.Core.Interfaces;
using RequestsManagementSystem.Dtos.EmployeeDtos;

namespace RequestsManagementSystem.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IConfiguration _configuration;
        private readonly IJWTService _JWT;

        public EmployeeService(IEmployeeRepository employeeRepository, IConfiguration configuration, IJWTService jWT)
        {
            _employeeRepository = employeeRepository;
            _configuration = configuration;
            _JWT = jWT;
        }

        public async Task<EmployeeDto> GetEmployeeDataAsync(int id)
        {
            // Validate employee credentials

            var employee = await _employeeRepository.GetEmployeeById(id) ?? throw new NullReferenceException("المستخدم غير موجود");
            employee.Manager = await _employeeRepository.GetEmployeeById(employee.ManagerId ?? 0);
            var resut = new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.Name,
                DepartmentName = employee.DepartmentName,
                DateOfEmployment = employee.DateOfEmployment,
                ManagerName= employee.Manager?.Name ?? "",
                RegularLeaveCount = float.Parse(_configuration["TotalRegularLeave"]!) - employee.RegularLeaveCount,
                CasualLeaveCount = int.Parse(_configuration["TotalCasualLeave"]!) - employee.CasualLeaveCount,
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
                EmployeeDto= new EmployeeDto
                {
                    EmployeeId = employee.EmployeeId,
                    EmployeeName = employee.Name,
                    DepartmentName = employee.DepartmentName,
                    DateOfEmployment = employee.DateOfEmployment,
                    ManagerName = employee.Manager?.Name ?? "",
                    RegularLeaveCount = float.Parse(_configuration["TotalRegularLeave"]!) - employee.RegularLeaveCount,
                    CasualLeaveCount = int.Parse(_configuration["TotalCasualLeave"]!) - employee.CasualLeaveCount,
                },
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
    }
}

