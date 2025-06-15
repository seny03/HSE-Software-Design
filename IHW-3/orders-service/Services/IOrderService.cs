using OrdersService.Controllers;
using OrdersService.Models;

namespace OrdersService.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(CreateOrderRequestDto requestDto);
    Task<List<Order>> GetOrdersAsync(Guid userId);
    Task<Order?> GetOrderAsync(Guid id);
    Task<Order?> GetOrderByUserIdAsync(Guid userId);
} 