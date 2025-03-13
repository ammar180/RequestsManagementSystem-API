using OfficeOpenXml;

namespace RequestsManagementSystem.Logic.Services
{
    public class AdminService : IAdminService
    {
        private readonly IEmployeeService _employeeService;

        public AdminService(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
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
    }
}
