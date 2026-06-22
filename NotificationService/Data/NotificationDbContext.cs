using Microsoft.EntityFrameworkCore;
using SmartBank.Notification.Models;

namespace SmartBank.Notification.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options) { }

        public DbSet<Models.Notification> Notifications => Set<Models.Notification>();
    }
}