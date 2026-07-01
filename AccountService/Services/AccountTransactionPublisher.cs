using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AccountService.Services
{
    public class AccountTransactionPublisher
    {
        public async Task PublishAccountEventAsync(int accountId, string eventType, decimal amount,decimal balance)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "account-to-transaction-queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var payload = new
            {
                AccountId = accountId,
                EventType = eventType ,
                Amount = amount,
                Balance = balance,
                OccurredAt = DateTime.UtcNow,
                // "AccountOpened" or "AccountClosed"
            };

            string json = JsonSerializer.Serialize(payload);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "account-to-transaction-queue",
                body: body
            );
        }
    }
}