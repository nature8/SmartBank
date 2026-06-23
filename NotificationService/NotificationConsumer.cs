using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using SmartBank.Notification.Data;

namespace SmartBank.Notification.Services
{
    public class NotificationConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public NotificationConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = await factory.CreateConnectionAsync(stoppingToken);
            using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(
                queue: "notification-queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken
            );

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                var data = JsonSerializer.Deserialize<NotificationMessage>(json);

                if (data != null)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

                    context.Notifications.Add(new Models.Notification
                    {
                        CustomerId = data.CustomerId,
                        Message = data.Message,
                        Type = data.Type
                    });

                    await context.SaveChangesAsync();
                }
            };

            await channel.BasicConsumeAsync(
                queue: "notification-queue",
                autoAck: true,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }

    public class NotificationMessage
    {
        public int CustomerId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}