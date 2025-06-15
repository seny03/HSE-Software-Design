namespace OrdersService.Models;
 
public record PaymentFailed
{
    public Guid OrderId { get; init; }
} 