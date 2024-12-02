﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Interfaces;
using RequestsManagementSystem.Dtos.EmployeeDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

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
            if (EmployeeDto.Password == null || EmployeeDto.Password == string.Empty)
            {
                return new UpdatePasswordResultDto
                {
                    Status = false,
                    message = "ادخل كلمه المرور",
                };
            }
            if (EmployeeDto.ConfirmPassword == null || EmployeeDto.ConfirmPassword == string.Empty)
            {
                return new UpdatePasswordResultDto
                {
                    Status = false,
                    message = "ادخل تاكيد كلمه المرور",
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
            string passregex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$";
            Regex regex = new Regex(passregex);
            if(!regex.IsMatch(EmployeeDto.Password))
            {
                return new UpdatePasswordResultDto
                {
                    Status = false,
                    message = "كلمه المرور ضعيفه",
                };
            }
            if(EmployeeDto.Password != EmployeeDto.ConfirmPassword)
            {
                return new UpdatePasswordResultDto
                {
                    Status = false,
                    message = "كلمه المرور غير مطابقه",
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

