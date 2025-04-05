using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RequestsManagementSystem.Core.Interfaces.IRepositories;
using RequestsManagementSystem.Core.Interfaces.IServices;
using RequestsManagementSystem.DTOs.api.EmployeeDtos;
using RequestsManagementSystem.DTOs.api.TransactionsDtos;
using RequestsManagementSystem.DTOs.ViewModels;

namespace RequestsManagementSystem.Logic.Services
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

        public async Task ImportEmployeesFromExcel(IFormFile excelFile)
        {
            // Read and parse the Excel file to extract employee data
            var employees = await ExtractEmployeesFromExcelAsync(excelFile);

            foreach (var employeeDto in employees)
            {

                var employee = await _employeeService.GetEmployeeByCodeAsync(employeeDto.Code);
                if (employee == null)
                {

                    continue;
                }


                if (employeeDto.RegularBalance > 0)
                {
                    var regularLeaveTransaction = new CreateTransactionDto
                    {
                        Title = "Leave",
                        Type = "AdditionalRegularLeave",
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(employeeDto.RegularBalance),
                        SubstituteEmployeeId = 0, // Assuming no substitute
                        EmployeeId = employee.EmployeeId
                    };
                    await _transactionService.AddTransactionAsync(regularLeaveTransaction);
                }


                if (employeeDto.CausalBalance > 0)
                {
                    var casualLeaveTransaction = new CreateTransactionDto
                    {
                        Title = "Leave",
                        Type = "AdditionalCasualLeave",
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(employeeDto.CausalBalance),
                        SubstituteEmployeeId = 0, // Assuming no substitute
                        EmployeeId = employee.EmployeeId
                    };
                    await _transactionService.AddTransactionAsync(casualLeaveTransaction);
                }
            }
        }

        public async Task<List<EmployeeExcelDto>> ExtractEmployeesFromExcelAsync(IFormFile file)
        {
            var employees = new List<EmployeeExcelDto>();

            if (file == null || file.Length <= 0)
                throw new ArgumentException("The uploaded file is empty.");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                        throw new ArgumentException("No worksheet found in the Excel file.");

                    var rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var employeeDto = new EmployeeExcelDto
                        {
                            Code = worksheet.Cells[row, 1].Text.Trim(),
                            Name = worksheet.Cells[row, 2].Text.Trim(),
                            RegularBalance = double.TryParse(worksheet.Cells[row, 3].Text.Trim(), out double regularBalance) ? regularBalance : 0,
                            CausalBalance = double.TryParse(worksheet.Cells[row, 4].Text.Trim(), out double causalBalance) ? causalBalance : 0
                        };

                        employees.Add(employeeDto);
                    }
                }
            }

            return employees;
        }
        public async Task<IEnumerable<EmployeeDashboardDto>> GetEmployeesDashboard(DateOnly? startDate, DateOnly? EndDate)
        {
            return (await _employeeService.GetEmployeesToExcelFormat(startDate,EndDate)).Select(x =>
            {
                return new EmployeeDashboardDto
                {
                    Name = x.Name,
                    Code = x.Code,
                    CausalBalance = x.CausalBalance,
                    RegularBalance = x.RegularBalance,
                };
            }).ToList();
        }

    }
}

