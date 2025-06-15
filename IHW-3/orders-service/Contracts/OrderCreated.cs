namespace OrdersService.Contracts;

public record OrderCreated
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime Timestamp { get; init; }
} 