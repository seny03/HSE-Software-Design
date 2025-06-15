using MassTransit;
using Microsoft.AspNetCore.SignalR;
using OrdersService.Contracts;
using OrdersService.Data;
using OrdersService.Hubs;
using OrdersService.Models;

namespace OrdersService.Consumers;

public class PaymentCompletedConsumer : IConsumer<OrdersService.Contracts.PaymentCompleted>
{
    private readonly OrdersDbContext _context;
    private readonly IHubContext<OrderStatusHub> _hubContext;

    public PaymentCompletedConsumer(OrdersDbContext context, IHubContext<OrderStatusHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<OrdersService.Contracts.PaymentCompleted> context)
    {
        var order = await _context.Orders.FindAsync(context.Message.OrderId);
        if (order != null)
        {
            order.Status = OrderStatus.Paid;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(order.Id.ToString()).SendAsync("OrderStatusChanged", order.Id, order.Status.ToString());
        }
    }
} 