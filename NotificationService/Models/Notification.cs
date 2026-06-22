using System;

namespace SmartBank.Notification.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string Message { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty; // e.g. "Deposit", "Withdraw", "Transfer", "Login"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}