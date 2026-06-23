using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using CustomerService.Data;
using CustomerService.Models;

namespace CustomerService.Services
{
    public class CustomerConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CustomerConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Console.WriteLine("[CustomerConsumer] Starting up...");

                var factory = new ConnectionFactory { HostName = "localhost" };

                using var connection = await factory.CreateConnectionAsync(stoppingToken);
                using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

                Console.WriteLine("[CustomerConsumer] Connected to RabbitMQ.");

                await channel.QueueDeclareAsync(
                    queue: "auth-to-customer-queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: stoppingToken
                );

                Console.WriteLine("[CustomerConsumer] Queue declared. Listening for messages...");

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        Console.WriteLine("[CustomerConsumer] Message received!");

                        var body = ea.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);

                        Console.WriteLine($"[CustomerConsumer] Raw message: {json}");

                        var data = JsonSerializer.Deserialize<NewUserMessage>(json);

                        if (data != null)
                        {
                            using var scope = _scopeFactory.CreateScope();
                            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                            var nameParts = data.FullName.Split(' ', 2);
                            var firstName = nameParts.Length > 0 ? nameParts[0] : data.FullName;
                            var lastName = nameParts.Length > 1 ? nameParts[1] : "";

                            context.Customers.Add(new Customer
                            {
                                UserId = data.UserId,
                                FirstName = firstName,
                                LastName = lastName,
                                Email = data.Email,
                                PhoneNumber = data.PhoneNumber,
                                Address = "",
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            });

                            await context.SaveChangesAsync();

                            Console.WriteLine("[CustomerConsumer] Customer saved successfully!");
                        }
                        else
                        {
                            Console.WriteLine("[CustomerConsumer] Deserialized data was null!");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[CustomerConsumer] ERROR processing message: {ex}");
                    }
                };

                await channel.BasicConsumeAsync(
                    queue: "auth-to-customer-queue",
                    autoAck: true,
                    consumer: consumer,
                    cancellationToken: stoppingToken
                );

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CustomerConsumer] FATAL ERROR in ExecuteAsync: {ex}");
            }
        }
    }

    public class NewUserMessage
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}