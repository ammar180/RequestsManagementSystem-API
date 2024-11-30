using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Interfaces;
using RequestsManagementSystem.Dtos.EmployeeDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RequestsManagementSystem.Services
{
    public class EmployeeService:IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IConfiguration _configuration;

        public EmployeeService(IEmployeeRepository employeeRepository, IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            _configuration = configuration;
        }

        public async Task<LoginResultDto> LoginAsync(LoginEmployeeDto loginEmployeeDto)
        {
            // Validate employee credentials

            var employee = await _employeeRepository.GetEmployeeById(loginEmployeeDto.EmployeeId);

            if (employee == null || employee.Password != loginEmployeeDto.Password)
            {
                throw new UnauthorizedAccessException("خطأ في كلمة لسر أو كود المستخدم");
            }

            var token = GenerateJwtToken(employee);

            return new LoginResultDto
            { 
                token= token,
                EmployeeDto=new EmployeeDto
                {
                    DepartmentName=employee.DepartmentName,
                    EmployeeId=employee.EmployeeId,
                    EmployeeName=employee.Name,
                },
                message="تم تسجيل الدخول بنجاح",
                Status=true
            };
        }
        public async Task<string> UpdatePasswordAsync(LoginEmployeeDto loginEmployeeDto)
        {
            // Validate employee credentials

            var employee = await _employeeRepository.GetEmployeeById(loginEmployeeDto.EmployeeId);

            if (employee == null)
            {
                throw new UnauthorizedAccessException("خطأ في كود المستخدم");
            }
            employee.Password = loginEmployeeDto.Password;
            bool response = await _employeeRepository.UpdateAsync(employee);
            if(response == true)
            {
                return "تم تحديث كلمه المرور بنجاح";
            }
            else
            {
                return "حدث خطأ اثناء عمليه التحديث";
            }
        }

        private string GenerateJwtToken(Employee employee)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, employee.Name),
                new Claim(ClaimTypes.NameIdentifier, employee.EmployeeId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

