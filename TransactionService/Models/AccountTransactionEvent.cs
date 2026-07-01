namespace SmartBank.TransactionService.Models
{
    public class AccountTransactionEvent
    {
        public int AccountId { get; set; }

        public decimal Amount { get; set; }

        public string TransactionType { get; set; } = string.Empty;
    }
}