using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Dtos.EmployeeDtos;

namespace RequestsManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResultDto>> Login(LoginEmployeeDto loginEmployeeDto)
        {
            try
            {
              var Response= await _employeeService.LoginAsync(loginEmployeeDto);
                return Ok(Response);
            }
            catch(UnauthorizedAccessException ex)
            {
                var result= new LoginResultDto
                {
                    message = ex.Message,
                    Status = false
                };
                return Unauthorized(result);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
