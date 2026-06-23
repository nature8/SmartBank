using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace SmartBank.Authentication.Services
{
    public class CustomerPublisher
    {
        public async Task PublishNewUserAsync(int userId, string fullName, string email, string phoneNumber)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "auth-to-customer-queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var payload = new
            {
                UserId = userId,
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber
            };

            string json = JsonSerializer.Serialize(payload);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "auth-to-customer-queue",
                body: body
            );
        }
    }
}