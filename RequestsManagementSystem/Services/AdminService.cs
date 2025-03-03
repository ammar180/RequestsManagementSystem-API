using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RequestsManagementSystem.Core.Entities;

namespace RequestsManagementSystem.Services
{
    public class AdminService : IAdminService
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITransactionService _transactionService;

        public AdminService(IEmployeeService employeeService, ITransactionService transactionService)
        {
            _employeeService = employeeService;
            _transactionService = transactionService;
        }




        public async Task<byte[]> ExportEmployeesToExcel(DateOnly? startDate, DateOnly? EndDate)
        {
            var employees = await _employeeService.GetEmployeesToExcelFormat(startDate, EndDate);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set license context
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");

                // Add headers
                worksheet.Cells[1, 1].Value = "Employee Code";
                worksheet.Cells[1, 2].Value = "Employee Name";
                worksheet.Cells[1, 3].Value = "Employee Casual Balance ";
                worksheet.Cells[1, 4].Value = "Employee Regular Balance";

                // Add data
                int row = 2;
                foreach (var emp in employees)
                {
                    worksheet.Cells[row, 1].Value = emp.Code;
                    worksheet.Cells[row, 2].Value = emp.Name;
                    worksheet.Cells[row, 3].Value = emp.CausalBalance;
                    worksheet.Cells[row, 4].Value = emp.RegularBalance;
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                return package.GetAsByteArray(); // Return as byte array
            }
        }



        public async Task<List<Employee>> ImportEmployeesFromExcel(IFormFile file)
        {
            var employeesList = new List<Employee>();

            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new Exception("No file uploaded");
                }

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using (var stream = file.OpenReadStream())
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Read()) 
                        {
                            if (reader.Depth == 0) continue;

                            var employee = new Employee
                            {
                                Code = reader.GetValue(0)?.ToString() ?? string.Empty,
                                Name = reader.GetValue(1)?.ToString() ?? string.Empty,
                                Password = reader.GetValue(2)?.ToString() ?? string.Empty,
                                DepartmentName = reader.GetValue(3)?.ToString() ?? string.Empty
                            };

                            employeesList.Add(employee);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return employeesList;
        }

    }
}

