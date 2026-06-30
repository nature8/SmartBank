using AccountService.Data;
using Microsoft.EntityFrameworkCore;    
using AccountService.Interfaces;
using AccountService.Models;

namespace AccountService.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDbContext _context;

        public AccountRepository(AccountDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> CreateAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == id);
        }

        public async Task<List<Account>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Accounts.Where(a => a.CustomerId == customerId).ToListAsync();
        }

        public async Task<bool> DepositAsync(int id, decimal amount)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null || !account.IsActive)
            {
                return false;
            }

            account.Balance += amount;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> WithdrawAsync(int id, decimal amount)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null || !account.IsActive || account.Balance < amount)
            {
                return false;
            }
            

            account.Balance -= amount;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CloseAccountAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return false;
            }

            account.IsActive = false;
            account.ClosedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}