namespace OrdersService.Models;
 
public record PaymentCompleted
{
    public Guid OrderId { get; init; }
} 