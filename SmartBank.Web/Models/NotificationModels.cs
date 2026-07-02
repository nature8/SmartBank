using System.ComponentModel.DataAnnotations;

namespace SmartBank.Web.Models
{
    public static class NotificationTypes
    {
        public const string Deposit = "Deposit";
        public const string Withdraw = "Withdraw";
        public const string Transfer = "Transfer";
        public const string Login = "Login";
        public const string General = "General";

        public static readonly string[] All = { General, Deposit, Withdraw, Transfer, Login };
    }

    /// <summary>Mirrors SmartBank.Notification.DTOs.CreateNotificationDto</summary>
    public class CreateNotificationDto
    {
        [Required(ErrorMessage = "Customer ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid customer ID.")]
        [Display(Name = "Customer ID")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(300, ErrorMessage = "Message cannot exceed 300 characters.")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please choose a notification type.")]
        public string Type { get; set; } = NotificationTypes.General;
    }

    /// <summary>Mirrors SmartBank.Notification.DTOs.NotificationResponseDto</summary>
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
