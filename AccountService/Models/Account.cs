namespace AccountService.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        //Reference to CustomerService
        public int CustomerId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ClosedAt { get; set; } = null;
    }
}