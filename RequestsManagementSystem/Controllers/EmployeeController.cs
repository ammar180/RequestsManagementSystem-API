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
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
		// update employee password
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
        // get employee profile
        [HttpGet("Profile{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeData(int id)
        {
            try
            {
                var result = await _employeeService.GetEmployeeDataAsync(id);
                return Ok(result);
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // reset employee balance
    }
}
