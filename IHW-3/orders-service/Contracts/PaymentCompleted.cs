namespace OrdersService.Contracts;
 
public record PaymentCompleted
{
    public Guid OrderId { get; init; }
} 