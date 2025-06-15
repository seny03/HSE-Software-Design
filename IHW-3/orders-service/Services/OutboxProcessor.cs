using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrdersService.Contracts;
using OrdersService.Data;
using OrdersService.Models;

namespace OrdersService.Services;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
                    var topicProducer = scope.ServiceProvider.GetRequiredService<ITopicProducer<Guid, OrderCreated>>();
                    var messages = await dbContext.OutboxMessages
                        .Where(m => m.SentAt == null)
                        .OrderBy(m => m.CreatedAt)
                        .Take(100)
                        .ToListAsync(stoppingToken);

                    foreach (var message in messages)
                    {
                        var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreated>(message.Content);
                        if (orderCreatedEvent != null)
                        {
                            await topicProducer.Produce(orderCreatedEvent.OrderId, orderCreatedEvent, stoppingToken);
                            message.SentAt = DateTime.UtcNow;
                            _logger.LogInformation("Sent OrderCreated event for order {OrderId}", orderCreatedEvent.OrderId);
                        }
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
} 