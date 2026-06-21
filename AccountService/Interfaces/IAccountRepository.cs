using AccountService.Models;

namespace AccountService.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> CreateAsync(Account account);
        Task<Account?> GetByIdAsync(int id);
        Task<List<Account>> GetByCustomerIdAsync(int customerId);
        Task<bool> CloseAccountAsync(int id);
    }
}