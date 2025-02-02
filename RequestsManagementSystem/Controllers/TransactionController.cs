using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestsManagementSystem.Dtos;
using RequestsManagementSystem.Dtos.TransactionsDtos;
using RequestsManagementSystem.Services;

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
        public async Task<ActionResult<BaseResponse>> PostTransaction(CreateTransactionDto transactionDto)
        {
            try
            {
                await _transactionService.AddTransactionAsync(transactionDto);
                return Ok(new BaseResponse
                {
                    Status = true,
                    Message = "تم ارسال الطلب بنجاح، برجاء اتظار رد المدير"
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
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("GetAllTransactionsByEmployeeId/{EmployeeId}")]
        public async Task<ActionResult<IEnumerable<GetTransactionByEmployeeDto>>> GetAllTransactionsByEmployeeId(int EmployeeId)
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactionsByEmployeeId(EmployeeId);
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
        public async Task<IActionResult> EditTransaction(int transactionId,UpdateTransactionDto transactionDto)
        {
            var result = await _transactionService.EditTransactionAsync(transactionId, transactionDto);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }


        [HttpDelete("RemoveTransactionAsync")]
        public async Task<IActionResult> RemoveTransactionAsync(int transactionId)
        {
            try
            {
                var result = await _transactionService.CancelTransactionAsync(transactionId);

                if (!result.Success)
                    return BadRequest(result.Message);

                return Ok(result.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ: {ex.Message}");
            }
        }
    }
}
