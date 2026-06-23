using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace SmartBank.TransactionService.Services
{
    public class NotificationPublisher
    {
        public async Task PublishNotificationAsync(int customerId, string message, string type)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "notification-queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var payload = new
            {
                CustomerId = customerId,
                Message = message,
                Type = type
            };

            string json = JsonSerializer.Serialize(payload);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "notification-queue",
                body: body
            );
        }
    }
}