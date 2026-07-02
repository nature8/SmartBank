using System.ComponentModel.DataAnnotations;

namespace SmartBank.Web.Models
{
    /// <summary>Mirrors SmartBank.TransactionService.DTOs.DepositDto</summary>
    public class DepositDto
    {
        [Required(ErrorMessage = "Account ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid account ID.")]
        [Display(Name = "Account ID")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 999999999, ErrorMessage = "Amount must be greater than zero.")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
    }

    /// <summary>Mirrors SmartBank.TransactionService.DTOs.WithdrawDto</summary>
    public class WithdrawDto
    {
        [Required(ErrorMessage = "Account ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid account ID.")]
        [Display(Name = "Account ID")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 999999999, ErrorMessage = "Amount must be greater than zero.")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
    }

    /// <summary>Mirrors SmartBank.TransactionService.DTOs.TransferDto</summary>
    public class TransferDto
    {
        [Required(ErrorMessage = "Source account ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid account ID.")]
        [Display(Name = "From account ID")]
        public int FromAccountId { get; set; }

        [Required(ErrorMessage = "Destination account ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid account ID.")]
        [Display(Name = "To account ID")]
        public int ToAccountId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 999999999, ErrorMessage = "Amount must be greater than zero.")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
    }

    /// <summary>Mirrors SmartBank.TransactionService.Models.Transaction (returned by the statement endpoint)</summary>
    public class TransactionDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int? DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
