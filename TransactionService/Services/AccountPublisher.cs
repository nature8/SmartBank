using RabbitMQ.Client;
using SmartBank.TransactionService.Models;
using System.Text;
using System.Text.Json;

namespace SmartBank.TransactionService.Services
{
    public class AccountPublisher
    {
        public async Task PublishAccountTransactionAsync(AccountTransactionEvent accountEvent)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "account-transaction-queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var json = JsonSerializer.Serialize(accountEvent);

            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "account-transaction-queue",
                body: body
            );
        }
    }
}