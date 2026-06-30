using AccountService.Models;

namespace AccountService.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> CreateAsync(Account account);
        Task<Account?> GetByIdAsync(int id);
        Task<List<Account>> GetByCustomerIdAsync(int customerId);
        Task<bool>DepositAsync(int id, decimal amount);
        Task<bool> WithdrawAsync(int id, decimal amount);
        Task<bool> CloseAccountAsync(int id);
    }
}