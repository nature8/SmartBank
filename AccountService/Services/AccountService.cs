using AccountService.DTOs;
using AccountService.Interfaces;
using AccountService.Models;

namespace AccountService.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly AccountTransactionPublisher _transactionPublisher;
        private readonly AccountPublisher _accountPublisher;

        public AccountService(
            IAccountRepository accountRepository,
            AccountTransactionPublisher transactionPublisher,
            AccountPublisher accountPublisher)
        {
            _accountRepository = accountRepository;
            _transactionPublisher = transactionPublisher;
            _accountPublisher = accountPublisher;
        }

        public async Task<AccountResponseDto?> CreateAccountAsync(CreateAccountDto createAccountDto)
        {
            var account = new Account
            {
                CustomerId = createAccountDto.CustomerId,
                AccountType = createAccountDto.AccountType,
                Balance = createAccountDto.InitialDeposit,
                AccountNumber = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 12).ToUpper()
            };

            var createdAccount = await _accountRepository.CreateAsync(account);

            await _transactionPublisher.PublishAccountEventAsync(account.AccountId, "AccountOpened", 0, account.Balance);
            await _accountPublisher.PublishAccountOpenedAsync(account.CustomerId, account.AccountId, account.AccountNumber);

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
            var result = await _accountRepository.CloseAccountAsync(id);

            if (result)
            {
                var account = await _accountRepository.GetByIdAsync(id);
                if (account == null)
                {
                    return false;
                }
                await _transactionPublisher.PublishAccountEventAsync(id, "AccountClosed", 0, account.Balance);
            }
            return result;
        }
    }
}