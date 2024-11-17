using Microsoft.IdentityModel.Tokens;
using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Interfaces;
using RequestsManagementSystem.Dtos.EmployeeDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RequestsManagementSystem.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IConfiguration _configuration;

        public EmployeeService(IEmployeeRepository employeeRepository, IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(LoginEmployeeDto loginEmployeeDto)
        {
            // Validate employee credentials
            var employee = await _employeeRepository.GetEmployeeById(loginEmployeeDto.EmployeeId);

            if (employee == null || employee.Password != loginEmployeeDto.Password)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var token = GenerateJwtToken(employee);
            return token;
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

