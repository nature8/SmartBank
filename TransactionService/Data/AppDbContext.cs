using Microsoft.EntityFrameworkCore;
using SmartBank.TransactionService.Models;

namespace SmartBank.TransactionService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
    }
}