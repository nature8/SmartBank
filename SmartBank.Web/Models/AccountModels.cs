using System.ComponentModel.DataAnnotations;

namespace SmartBank.Web.Models
{
    public static class AccountTypes
    {
        public const string Savings = "Savings";
        public const string Current = "Current";
        public const string Fixed = "Fixed Deposit";

        public static readonly string[] All = { Savings, Current, Fixed };
    }

    /// <summary>Mirrors AccountService.DTOs.CreateAccountDto</summary>
    public class CreateAccountDto
    {
        [Required(ErrorMessage = "Customer ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be a positive number.")]
        [Display(Name = "Customer ID")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please choose an account type.")]
        [Display(Name = "Account type")]
        public string AccountType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Initial deposit is required.")]
        [Range(0, 999999999, ErrorMessage = "Initial deposit cannot be negative.")]
        [Display(Name = "Initial deposit")]
        [DataType(DataType.Currency)]
        public decimal InitialDeposit { get; set; }
    }

    /// <summary>Mirrors AccountService.DTOs.AccountResponseDto</summary>
    public class AccountResponseDto
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
    }

    public class CloseAccountDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid account ID.")]
        [Display(Name = "Account ID")]
        public int AccountId { get; set; }
    }
}
