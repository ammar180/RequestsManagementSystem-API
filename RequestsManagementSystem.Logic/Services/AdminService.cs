using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RequestsManagementSystem.Core.Enums;
using RequestsManagementSystem.Core.Interfaces.IRepositories;
using RequestsManagementSystem.Core.Interfaces.IServices;
using RequestsManagementSystem.DTOs.api.EmployeeDtos;
using RequestsManagementSystem.DTOs.api.TransactionsDtos;
using RequestsManagementSystem.Core.Entities;
namespace RequestsManagementSystem.Logic.Services
{
    public class AdminService : IAdminService
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITransactionService _transactionService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IEmployeeRepository _employeeRepo;
        public AdminService(IEmployeeService employeeService, ITransactionService transactionService, ITransactionRepository transactionRepository, IEmployeeRepository employeeRepository)
        {
            _employeeService = employeeService;
            _transactionService = transactionService;
            _transactionRepository = transactionRepository;
            _employeeRepo = employeeRepository;
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
        public async Task<bool> AddAdminTransactionAsync(CreateTransactionDto transactionDto)
        {
            try
            {
                var transaction = new Transaction
                {
                    StartDate = transactionDto.StartDate,
                    EndDate = transactionDto.EndDate,
                    SubstituteEmployeeId = transactionDto.SubstituteEmployeeId,
                    Itinerary = transactionDto.Itinerary,
                    EmployeeId = transactionDto.EmployeeId,
                    Status = TransactionStatus.Approved,
                };
                // validate transaction id
                transaction.Type = _transactionRepository.GetTransactionTypeIdByName(transactionDto.Type) ?? throw new InvalidOperationException("لم نستطيع تحديد نوع الطلب!");
                // Add the transaction to database via repository
                await _transactionRepository.AddTransactionAsync(transaction);

                return true;
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException("حدث خطأ أثناء حفظ الطلب، ربما ادخلت موظف غير متاح");
            }
        }
        public async Task ImportEmployeesFromExcel(IFormFile excelFile, bool isCasualImportAllowed)
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
                        SubstituteEmployeeId = null, // Assuming no substitute
                        EmployeeId = employee.EmployeeId
                    };
                    await AddAdminTransactionAsync(regularLeaveTransaction);
                }

                if(isCasualImportAllowed)
                {
                    if (employeeDto.CausalBalance > 0)
                    {
                        var casualLeaveTransaction = new CreateTransactionDto
                        {
                            Title = "Leave",
                            Type = "AdditionalCasualLeave",
                            StartDate = DateTime.Now,
                            EndDate = DateTime.Now.AddDays(employeeDto.CausalBalance),
                            SubstituteEmployeeId = null, // Assuming no substitute
                            EmployeeId = employee.EmployeeId
                        };
                        await AddAdminTransactionAsync(casualLeaveTransaction);
                    }
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
                            CausalBalance = double.TryParse(worksheet.Cells[row, 3].Text.Trim(), out double causalBalance) ? causalBalance : 0,
                            RegularBalance = double.TryParse(worksheet.Cells[row, 4].Text.Trim(), out double regularBalance) ? regularBalance : 0,
                        };

                        employees.Add(employeeDto);
                    }
                }
            }

            return employees;
        }



    }
}

