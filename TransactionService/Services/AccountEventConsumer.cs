using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using SmartBank.TransactionService.Data;
using SmartBank.TransactionService.Models;

namespace SmartBank.TransactionService.Services
{
    public class AccountEventConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AccountEventConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Console.WriteLine("[AccountEventConsumer] Starting up...");

                var factory = new ConnectionFactory { HostName = "localhost" };

                using var connection = await factory.CreateConnectionAsync(stoppingToken);
                using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

                Console.WriteLine("[AccountEventConsumer] Connected to RabbitMQ.");

                await channel.QueueDeclareAsync(
                    queue: "account-to-transaction-queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: stoppingToken
                );

                Console.WriteLine("[AccountEventConsumer] Queue declared. Listening for messages...");

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        Console.WriteLine("[AccountEventConsumer] Message received!");

                        var body = ea.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);

                        Console.WriteLine($"[AccountEventConsumer] Raw message: {json}");

                        var data = JsonSerializer.Deserialize<AccountEventMessage>(json);

                        if (data != null)
                        {
                            using var scope = _scopeFactory.CreateScope();
                            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                            context.Transactions.Add(new Transaction
                            {
                                AccountId = data.AccountId,
                                Amount = 0,
                                Type = data.EventType,
                                CreatedAt = DateTime.UtcNow
                            });

                            await context.SaveChangesAsync();

                            Console.WriteLine("[AccountEventConsumer] Transaction record saved successfully!");
                        }
                        else
                        {
                            Console.WriteLine("[AccountEventConsumer] Deserialized data was null!");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[AccountEventConsumer] ERROR processing message: {ex}");
                    }
                };

                await channel.BasicConsumeAsync(
                    queue: "account-to-transaction-queue",
                    autoAck: true,
                    consumer: consumer,
                    cancellationToken: stoppingToken
                );

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountEventConsumer] FATAL ERROR in ExecuteAsync: {ex}");
            }
        }
    }

    public class AccountEventMessage
    {
        public int AccountId { get; set; }
        public string EventType { get; set; } = string.Empty;
    }
}