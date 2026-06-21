using AccountService.DTOs;
using AccountService.Interfaces;
using AccountService.Models;

namespace AccountService.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<AccountResponseDto?> CreateAccountAsync(CreateAccountDto createAccountDto)
        {
            var account = new Account
            {
                CustomerId = createAccountDto.CustomerId,
                AccountType = createAccountDto.AccountType,
                Balance = createAccountDto.InitialDeposit,
                // Generate a unique account number
                AccountNumber = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 12).ToUpper()
            }; 

            var createdAccount = await _accountRepository.CreateAsync(account);

            

            return new AccountResponseDto
            {
                AccountId = account.AccountId,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                IsActive = account.IsActive
            };
        }

        public async Task<AccountResponseDto?> GetAccountAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                return null;
            }

            return new AccountResponseDto
            {
                AccountId = account.AccountId,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                IsActive = account.IsActive
            };
        }

        public async Task<List<AccountResponseDto>> GetCustomerAccountsAsync(int customerId)
        {
            var accounts = await _accountRepository.GetByCustomerIdAsync(customerId);
            return accounts.Select(account => new AccountResponseDto
            {
                AccountId = account.AccountId,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                IsActive = account.IsActive
            }).ToList();
        }

        public async Task<bool> CloseAccountAsync(int id)
        {
            return await _accountRepository.CloseAccountAsync(id);
        }


    }

}