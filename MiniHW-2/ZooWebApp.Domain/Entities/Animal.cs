using ZooWebApp.Domain.Common;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Domain.Events;
using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Domain.Entities;

public class Animal : EntityBase, IAggregateRoot
{
    public Species Species { get; private set; }
    public string Name { get; private set; } = string.Empty; // Initialize to satisfy non-null constraint
    public DateTime BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public FoodType FavoriteFood { get; private set; }
    public HealthStatus HealthStatus { get; private set; }
    public Enclosure? Enclosure { get; private set; }
    
    private readonly List<FeedingSchedule> _feedingSchedules = new();
    public IReadOnlyCollection<FeedingSchedule> FeedingSchedules => _feedingSchedules.AsReadOnly();

    private Animal() { }

    public Animal(Species species, string name, DateTime birthDate, Gender gender, FoodType favoriteFood)
    {
        Species = species;
        Name = name;
        BirthDate = birthDate;
        Gender = gender;
        FavoriteFood = favoriteFood;
        HealthStatus = HealthStatus.Healthy;
    }

    public void Feed(DateTime feedingTime)
    {
        if (HealthStatus == HealthStatus.Sick)
            throw new InvalidOperationException("Cannot feed a sick animal");
        
        _feedingSchedules.Add(new FeedingSchedule(
            Time: feedingTime.TimeOfDay,
            FoodItems: new List<FoodType> { FavoriteFood },
            Notes: $"Regular feeding at {feedingTime}"));
        
        AddDomainEvent(new FeedingTimeEvent 
        { 
            AnimalId = this.Id,
            AnimalName = this.Name,
            FeedingTime = feedingTime.TimeOfDay,
            FoodItems = new List<FoodType> { FavoriteFood }
        });
    }

    public void Treat()
    {
        HealthStatus = HealthStatus.Healthy;
    }

    public void MoveToEnclosure(Enclosure enclosure)
    {
        // Store previous enclosure name before changing it
        string previousEnclosureName = Enclosure?.Name ?? "None";
        
        // Clear previous enclosure if exists
        if (Enclosure != null)
            Enclosure = null;
        
        // Set the new enclosure
        Enclosure = enclosure;
        
        // Create the domain event with the stored previous name
        AddDomainEvent(new AnimalTransferredEvent 
        { 
            AnimalId = Id,
            PreviousEnclosure = previousEnclosureName,
            NewEnclosure = enclosure.Name,
            TransferDate = DateTime.UtcNow
        });
    }
    
    public void Update(Species species, string name, DateTime birthDate, Gender gender, FoodType favoriteFood, HealthStatus healthStatus)
    {
        Species = species;
        Name = name;
        BirthDate = birthDate;
        Gender = gender;
        FavoriteFood = favoriteFood;
        HealthStatus = healthStatus;
    }
}
