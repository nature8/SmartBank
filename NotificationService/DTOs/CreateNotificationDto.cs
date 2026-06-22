namespace SmartBank.Notification.DTOs
{
    public class CreateNotificationDto
    {
        public int CustomerId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}