namespace SmartBank.TransactionService.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public int? DestinationAccountId { get; set; }

        public decimal Amount { get; set; }

        public string Type { get; set; } = string.Empty; // Deposit, Withdraw, Transfer

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
