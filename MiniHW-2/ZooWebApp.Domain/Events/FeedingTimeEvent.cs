using ZooWebApp.Domain.Common.Interfaces;
using ZooWebApp.Domain.Enums;

namespace ZooWebApp.Domain.Events;

public class FeedingTimeEvent : IDomainEvent
{
    public int AnimalId { get; init; }
    public string AnimalName { get; init; } = string.Empty;
    public TimeSpan FeedingTime { get; init; }
    public List<FoodType> FoodItems { get; init; } = new();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
