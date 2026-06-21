using AccountService.DTOs;

namespace AccountService.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResponseDto?> CreateAccountAsync(CreateAccountDto createAccountDto);
        Task<AccountResponseDto?> GetAccountAsync(int id);
        Task<List<AccountResponseDto>> GetCustomerAccountsAsync(int customerId);
        Task<bool> CloseAccountAsync(int id);
    }
}