using Microsoft.AspNetCore.Mvc;
using RequestsManagementSystem.Dtos.EmployeeDtos;
using RequestsManagementSystem.Services;
using System.Threading.Tasks;

namespace RequestsManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("export-employees")]
        public async Task<IActionResult> ExportEmployeesToExcel(
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null)
        {
            try
            {
                // Get the Excel file bytes from the service
                byte[] fileBytes = await _adminService.ExportEmployeesToExcel(startDate, endDate);

                // Set file name with current date
                string fileName = $"Employees_{DateTime.Now:dd/MM/yyyy}.xlsx";

                // Return the file as a download
                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                // Handle any errors and return appropriate response
                return StatusCode(500, $"An error occurred while generating the Excel file: {ex.Message}");
            }
        }
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportEmployeesFromExcel([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                List<EmployeeExcelDto> employees = await _adminService.ImportEmployeesFromExcel(file);

                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}