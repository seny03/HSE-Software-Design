namespace OrdersService.Contracts;
 
public record PaymentFailed
{
    public Guid OrderId { get; init; }
} 