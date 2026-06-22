using Microsoft.EntityFrameworkCore;
using SmartBank.Notification.Data;
using SmartBank.Notification.DTOs;

namespace SmartBank.Notification.Services
{
    public class NotificationAppService
    {
        private readonly NotificationDbContext _context;

        public NotificationAppService(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationResponseDto> CreateAsync(CreateNotificationDto dto)
        {
            var notification = new Models.Notification
            {
                CustomerId = dto.CustomerId,
                Message = dto.Message,
                Type = dto.Type
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return new NotificationResponseDto
            {
                Id = notification.Id,
                CustomerId = notification.CustomerId,
                Message = notification.Message,
                Type = notification.Type,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead
            };
        }

        public async Task<List<NotificationResponseDto>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Notifications
                .Where(n => n.CustomerId == customerId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponseDto
                {
                    Id = n.Id,
                    CustomerId = n.CustomerId,
                    Message = n.Message,
                    Type = n.Type,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                })
                .ToListAsync();
        }
    }
}