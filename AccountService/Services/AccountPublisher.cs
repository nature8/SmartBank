using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AccountService.Services
{
    public class AccountPublisher
    {
        public async Task PublishAccountOpenedAsync(int customerId, int accountId, string accountNumber)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "account-to-customer-queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var payload = new
            {
                CustomerId = customerId,
                AccountId = accountId,
                AccountNumber = accountNumber
            };

            string json = JsonSerializer.Serialize(payload);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "account-to-customer-queue",
                body: body
            );
        }
    }
}