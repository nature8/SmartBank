using AccountService.DTOs;
using AccountService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountDto createAccountDto)
        {
            var result = await _accountService.CreateAccountAsync(createAccountDto);
            if (result == null) return BadRequest("Failed to create account.");
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _accountService.GetAccountAsync(id);
            if (result == null) return NotFound("Account not found.");
            return Ok(result);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerAccounts(int customerId)
        {
            var result = await _accountService.GetCustomerAccountsAsync(customerId);
            return Ok(result);
        }

        [HttpPost("close")]
        public async Task<IActionResult> Close([FromBody] CloseAccountDto closeAccountDto)
        {
            var success = await _accountService.CloseAccountAsync(closeAccountDto.AccountId);
            if (!success) return BadRequest("Failed to close account.");
            return Ok("Account closed successfully.");
        }
    }
}