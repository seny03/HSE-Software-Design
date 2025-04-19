using ZooWebApp.Domain.Common.Interfaces;

namespace ZooWebApp.Domain.Common;

public abstract class EntityBase : IEntity
{
    public int Id { get; protected set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

public interface IEntity
{
    int Id { get; }
}

public interface IAggregateRoot { }
