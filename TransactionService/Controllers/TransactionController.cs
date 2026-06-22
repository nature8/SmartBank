using Microsoft.AspNetCore.Mvc;
using SmartBank.TransactionService.DTOs;
using SmartBank.TransactionService.Services;

namespace SmartBank.TransactionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionAppService _service;

        public TransactionController(TransactionAppService service)
        {
            _service = service;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(DepositDto dto)
        {
            await _service.Deposit(dto);
            return Ok("Deposit Successful");
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(WithdrawDto dto)
        {
            await _service.Withdraw(dto);
            return Ok("Withdraw Successful");
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferDto dto)
        {
            await _service.Transfer(dto);
            return Ok("Transfer Successful");
        }

        [HttpGet("statement/{accountId}")]
        public async Task<IActionResult> GetStatement(int accountId)
        {
            var result = await _service.GetStatement(accountId);
            return Ok(result);
        }
    }
}