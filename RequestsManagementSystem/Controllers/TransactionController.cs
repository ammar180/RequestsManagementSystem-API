using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestsManagementSystem.Core.Interfaces.IServices;
using RequestsManagementSystem.DTOs.api;
using RequestsManagementSystem.DTOs.api.TransactionsDtos;

namespace RequestsManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("PostTransaction")]
        public async Task<ActionResult<BaseResponse>> PostTransaction([FromBody] CreateTransactionDto transactionDto)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault(); // Get the first validation error message

                return BadRequest(new BaseResponse
                {
                    Status = false,
                    Message = errorMessage ?? "حدث خطأ في التحقق من صحة البيانات"
                });
            }

            try
            {
                await _transactionService.AddTransactionAsync(transactionDto);
                return Ok(new BaseResponse
                {
                    Status = true,
                    Message = "تم ارسال الطلب بنجاح، برجاء انتظار رد المدير"
                });
            }
            catch (InvalidOperationException ex)
            {
                return Ok(new BaseResponse
                {
                    Status = false,
                    Message = ex.Message
                });
            }
            catch (Exception)
            {
                return Ok(new BaseResponse
                {
                    Status = false,
                    Message = "عذرا، حدث خطأ غير متوقع"
                });
            }
        }

        [HttpGet("GetStaffTransactions/{managerId}")]
        public async Task<ActionResult<IEnumerable<StaffTransactionDto>>> GetStaffTransaction(int managerId)
        {
            try
            {
                var transaction = await _transactionService.GetStaffTransaction(managerId);

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet("GetTransactionDetails/{transactionId}")]
        public async Task<ActionResult<TransactionDto>> GetTransactionDetails(int transactionId)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByIdAsync(transactionId);

                return Ok(transaction);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("GetAllTransactionsByEmployeeId/{employeeId}")]
        public async Task<ActionResult<IEnumerable<GetTransactionByEmployeeDto>>> GetAllTransactionsByEmployeeId(int employeeId)
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactionsByEmployeeId(employeeId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPatch("{id}/seen")]
        public async Task<IActionResult> UpdateSeenStatus(int id, string whoSeen)
        {
            try
            {
                await _transactionService.SetSeenStatus(id, whoSeen);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("EditTransaction")]
        public async Task<IActionResult> EditTransaction(int transactionId, UpdateTransactionDto transactionDto)
        {
            var result = await _transactionService.EditTransactionAsync(transactionId, transactionDto);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }


        [HttpDelete("CanelTransaction/{transactionId}")]
        public async Task<IActionResult> RemoveTransactionAsync(int transactionId)
        {
            try
            {
                var (Success, Message) = await _transactionService.CancelTransactionAsync(transactionId);

                if (!Success)
                    return BadRequest(Message);

                return Ok(Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ: {ex.Message}");
            }
        }
        [HttpPut("SetStatus/{id}")]
        public async Task<IActionResult> SetTransactionStatus(int id, UpdateTransactionStatusDto request)
        {
            try
            {
                var result = await _transactionService.UpdateTransactionStatusAsync(id, request);
                return Ok(new BaseResponse { Status = true, Message = result });
            }
            catch (InvalidOperationException ex)
            {
                return Ok(new BaseResponse { Status = false, Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new BaseResponse { Status = false, Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new BaseResponse { Status = false, Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new BaseResponse { Status = false, Message = "خطأ غير متوقع" });
            }
        }

        [HttpGet("EmployeeReport/{EmployeeId}")]
        public async Task<IActionResult> EmployeeReport(int EmployeeId, [FromQuery] string p_type, [FromQuery] DateTime? StartDate, [FromQuery] DateTime? EndDate)
        {
            try
            {
                var result = await _transactionService.EmployeeReport(EmployeeId, p_type, StartDate, EndDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
