namespace AccountService.DTOs
{
    public class CreateAccountDto
    {
        public int CustomerId { get; set; }
        public string AccountType { get; set; } = string.Empty;
        public decimal InitialDeposit { get; set; }
    }
}