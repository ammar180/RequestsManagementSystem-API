using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestsManagementSystem.Dtos.TransactionsDtos;
using RequestsManagementSystem.Services;

namespace RequestsManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }


        [HttpPost("PostTransaction")]
        public async Task<ActionResult<CreateTransactionResponseDto>> PostTransaction(CreateTransactionDto transactionDto)
        {
            if (transactionDto == null)
            {
                return BadRequest(new CreateTransactionResponseDto
                {
                    Status = false,
                    Message = "Invalid data."
                });
            }

            bool isAdded = await _transactionService.AddTransactionAsync(transactionDto);

            if (isAdded)
            {
                return Ok(new CreateTransactionResponseDto
                {
                    Status = true,
                    Message = "تم ارسال الطلب بنجاح، برجاء اتظار رد المدير"
                });
            }
            else
            {
                return BadRequest(new CreateTransactionResponseDto
                {
                    Status = false,
                    Message = "حدث خطأ أثناء ارسال الطلب."
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
    }
}
