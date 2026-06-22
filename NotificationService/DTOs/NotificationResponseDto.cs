using System;

namespace SmartBank.Notification.DTOs
{
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