using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using AccountService.DTOs;
using AccountService.Interfaces;


namespace AccountService.Services
{
    public class AccountTransactionConsumer:BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
       

        public AccountTransactionConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = await factory.CreateConnectionAsync(stoppingToken);
            using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(
                queue: "account-transaction-queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken
            );

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
               var json = Encoding.UTF8.GetString(ea.Body.ToArray());
               var transaction = JsonSerializer.Deserialize<AccountTransactionEvent>(json);


                if (transaction == null)
                {
                    return;
                }

                using var scope = _scopeFactory.CreateScope();
                var repo=scope.ServiceProvider.GetRequiredService<IAccountRepository>();

                var account = await repo.GetByIdAsync(transaction.AccountId);
                if (account == null)
                {
                    return;
                }
                if (transaction.TransactionType == "Deposit")
                {
                    account.Balance += transaction.Amount;
                }
                else if (transaction.TransactionType == "Withdrawal")
                {
                    account.Balance -= transaction.Amount;
                }   
                await repo.UpdateAsync(account);

                
            };

            await channel.BasicConsumeAsync(
                queue: "account-transaction-queue",
                autoAck: true,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}