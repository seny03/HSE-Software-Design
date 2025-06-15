using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OrdersService.Contracts;
using OrdersService.Data;
using OrdersService.Models;

namespace OrdersService.Services;

public class OrderService : IOrderService
{
    private readonly OrdersDbContext _context;
    private readonly IProductService _productService;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        OrdersDbContext context,
        IProductService productService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<OrderService> logger)
    {
        _context = context;
        _productService = productService;
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequestDto requestDto)
    {
        
        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        foreach (var item in requestDto.Items)
        {
            var product = await _productService.GetProductByIdAsync(item.ProductId);
            if (product == null)
            {
                throw new Exception($"Product with ID {item.ProductId} not found");
            }

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = product.Price
            };

            orderItems.Add(orderItem);
            totalAmount += orderItem.Price * orderItem.Quantity;
        }

        
        var order = new Order
        {
            UserId = requestDto.UserId,
            Items = orderItems,
            TotalAmount = totalAmount,
            Status = OrderStatus.Created,
            CreatedAt = DateTime.UtcNow
        };

        
        try
        {
            var paymentsServiceUrl = _configuration["PaymentsServiceUrl"];
            var response = await _httpClient.PostAsync(
                $"{paymentsServiceUrl}/payments/account/withdraw?userId={requestDto.UserId}&amount={totalAmount}", 
                null);

            if (!response.IsSuccessStatusCode)
            {
                order.Status = OrderStatus.Failed;
                _logger.LogError("Failed to withdraw funds: {StatusCode}", response.StatusCode);
            }
            else
            {
                order.Status = OrderStatus.Paid;
            }
        }
        catch (Exception ex)
        {
            order.Status = OrderStatus.Failed;
            _logger.LogError(ex, "Error occurred while processing payment");
        }

        
        var orderCreatedEvent = new OrderCreated
        {
            OrderId = order.Id,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,
            Timestamp = order.CreatedAt
        };

        var outboxMessage = new OutboxMessage
        {
            Type = "OrderCreated",
            Content = JsonSerializer.Serialize(orderCreatedEvent)
        };

        
        await using var transaction = await _context.Database.BeginTransactionAsync();

        _context.Orders.Add(order);
        _context.OutboxMessages.Add(outboxMessage);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return order;
    }

    public async Task<List<Order>> GetOrdersAsync(Guid userId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderAsync(Guid id)
    {
        return await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> GetOrderByUserIdAsync(Guid userId)
    {
        return await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.UserId == userId);
    }
} 