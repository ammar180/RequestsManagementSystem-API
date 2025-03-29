using Microsoft.AspNetCore.Mvc;
using RequestsManagementSystem.Core.Interfaces.IServices;

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
        [HttpPost("import-employees")]
        public async Task<IActionResult> ImportEmployeesFromExcel(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                return BadRequest("Please upload a valid Excel file.");

            try
            {
                await _adminService.ImportEmployeesFromExcel(file);

                // Process the list of employees as needed, e.g., save to the database

                return Ok("Employees imported successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while importing the Excel file: {ex.Message}");
            }
        }

    }
}