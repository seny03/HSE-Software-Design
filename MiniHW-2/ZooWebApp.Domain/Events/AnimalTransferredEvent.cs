using ZooWebApp.Domain.Common.Interfaces;

namespace ZooWebApp.Domain.Events;

public class AnimalTransferredEvent : IDomainEvent
{
    public int AnimalId { get; init; }
    public string PreviousEnclosure { get; init; } = "None";
    public string NewEnclosure { get; init; } = "None";
    public DateTime TransferDate { get; init; } = DateTime.UtcNow;
    public DateTime OccurredOn => TransferDate;
}
