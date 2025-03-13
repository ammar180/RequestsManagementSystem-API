using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.DTOs.api.EmployeeDtos;
using RequestsManagementSystem.Services;

namespace RequestsManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IJWTService _jWTService;
        public EmployeeController(IEmployeeService employeeService, IJWTService jWT)
        {
            _employeeService = employeeService;
            _jWTService = jWT;
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
                    Message = ex.Message,
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
        [HttpGet("Profile/{id}")]
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
        [HttpGet("NewToken")]
        [AllowAnonymous]
        public ActionResult<string> GetNewToken(string refreshToken)
        {
            try
            {
                if(string.IsNullOrEmpty(refreshToken) || _jWTService.IsTokenExpired(refreshToken))
                {
                    return Unauthorized();
                }

                var payload = _jWTService.GetEmployeePayloadFromToken(refreshToken);

                var token = _jWTService.GenerateJwtToken(payload);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // reset employee balance


        // Get Employees By Department Name

        [HttpGet("GetEmployeesByDepartmentName/{departmentName}")]
        public async Task<IActionResult> GetEmployeesByDepartmentName(string departmentName)
        {
            try
            {
                var employees = await _employeeService.GetEmployeesAsync(departmentName);
                return Ok(employees);
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
