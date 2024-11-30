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

        [HttpPost("Login")]
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
        [HttpPost("UpdatePassword")]
        [Authorize]
        public async Task<ActionResult<UpdatePasswordResultDto>> UpdatePassword(UpdatePasswordEmployeeDto EmployeeDto)
        {
            try
            {
                var Response = await _employeeService.UpdatePasswordAsync(EmployeeDto);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new UpdatePasswordResultDto
                {
                    Status = false,
                    message = ex.Message
                });
            }
        }

    }
}
