namespace PaymentsService.Contracts;

public record PaymentFailed
{
    public Guid OrderId { get; init; }
} 