using SmartBank.TransactionService.Data;
using SmartBank.TransactionService.DTOs;
using SmartBank.TransactionService.Models;
using Microsoft.EntityFrameworkCore;

namespace SmartBank.TransactionService.Services
{
    public class TransactionAppService
    {
        private readonly AppDbContext _context;
        private readonly NotificationPublisher _notificationPublisher;
        private readonly AccountPublisher _accountPublisher;

        public TransactionAppService(AppDbContext context, 
            NotificationPublisher notificationPublisher,
            AccountPublisher accountPublisher)
        {
            _context = context;
            _notificationPublisher = notificationPublisher;
            _accountPublisher = accountPublisher;
        }

        public async Task Deposit(DepositDto dto)
        {
            _context.Transactions.Add(new Transaction
            {
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Type = "Deposit",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            // Publish event to Account Service
            await _accountPublisher.PublishAccountTransactionAsync(
                new AccountTransactionEvent
                {
                    AccountId = dto.AccountId,
                    Amount = dto.Amount,
                    TransactionType = "Deposit"
                });

            // Publish Notification
            await _notificationPublisher.PublishNotificationAsync(
                dto.AccountId,
                $"Deposit of {dto.Amount} successful",
                "Deposit"
            );
        }

        public async Task Withdraw(WithdrawDto dto)
        {
            _context.Transactions.Add(new Transaction
            {
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Type = "Withdraw",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            // Publish event to Account Service
            await _accountPublisher.PublishAccountTransactionAsync(
                new AccountTransactionEvent
                {
                    AccountId = dto.AccountId,
                    Amount = dto.Amount,
                    TransactionType = "Withdraw"
                });

            // Publish Notification
            await _notificationPublisher.PublishNotificationAsync(
                dto.AccountId,
                $"Withdrawal of {dto.Amount} successful",
                "Withdraw"
            );
        }

        public async Task Transfer(TransferDto dto)
        {
            _context.Transactions.Add(new Transaction
            {
                AccountId = dto.FromAccountId,
                DestinationAccountId = dto.ToAccountId,
                Amount = dto.Amount,
                Type = "Transfer",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            await _notificationPublisher.PublishNotificationAsync(
                dto.FromAccountId,
                $"Transfer of {dto.Amount} to account {dto.ToAccountId} successful",
                "Transfer"
            );
        }

      public async Task<List<Transaction>> GetStatement(int accountId)
        {
            return await _context.Transactions
                .Where(t => t.AccountId == accountId || t.DestinationAccountId == accountId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}