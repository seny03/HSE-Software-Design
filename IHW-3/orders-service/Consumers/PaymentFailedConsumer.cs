using MassTransit;
using Microsoft.AspNetCore.SignalR;
using OrdersService.Contracts;
using OrdersService.Data;
using OrdersService.Hubs;
using OrdersService.Models;

namespace OrdersService.Consumers;

public class PaymentFailedConsumer : IConsumer<OrdersService.Contracts.PaymentFailed>
{
    private readonly OrdersDbContext _context;
    private readonly IHubContext<OrderStatusHub> _hubContext;

    public PaymentFailedConsumer(OrdersDbContext context, IHubContext<OrderStatusHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<OrdersService.Contracts.PaymentFailed> context)
    {
        var order = await _context.Orders.FindAsync(context.Message.OrderId);
        if (order != null)
        {
            order.Status = OrderStatus.Failed;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(order.Id.ToString()).SendAsync("OrderStatusChanged", order.Id, order.Status.ToString());
        }
    }
} 