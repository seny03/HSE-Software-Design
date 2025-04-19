using ZooWebApp.Domain.Enums;
using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Presentation.Models;

public class AnimalDto
{
    public int Id { get; set; }
    public Species Species { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public FoodType FavoriteFood { get; set; }
    public HealthStatus HealthStatus { get; set; }
    public EnclosureDto? Enclosure { get; set; }
}

public class EnclosureDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public int CurrentOccupancy { get; set; }
}

public class CreateAnimalDto
{
    public Species Species { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public FoodType FavoriteFood { get; set; }
}

public class UpdateAnimalDto
{
    public Species Species { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public FoodType FavoriteFood { get; set; }
    public HealthStatus HealthStatus { get; set; }
}
