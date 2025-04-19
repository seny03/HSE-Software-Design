using Moq;
using Xunit;
using ZooWebApp.Application.Services;
using ZooWebApp.Domain.Common.Interfaces;
using ZooWebApp.Domain.Entities;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Application.Tests.Services;

public class ZooStatisticsServiceTests
{
    private readonly Mock<IAnimalRepository> _mockAnimalRepository;
    private readonly Mock<IEnclosureRepository> _mockEnclosureRepository;
    private readonly ZooStatisticsService _statisticsService;

    public ZooStatisticsServiceTests()
    {
        _mockAnimalRepository = new Mock<IAnimalRepository>();
        _mockEnclosureRepository = new Mock<IEnclosureRepository>();
        _statisticsService = new ZooStatisticsService(_mockAnimalRepository.Object, _mockEnclosureRepository.Object);
    }

    [Fact]
    public async Task GetTotalAnimalCount_ReturnsCorrectCount()
    {
        // Arrange
        var animals = new List<Animal>
        {
            new(Species.Lion, "Leo", new DateTime(2020, 1, 1), Gender.Male, FoodType.Meat),
            new(Species.Tiger, "Tony", new DateTime(2019, 5, 15), Gender.Male, FoodType.Meat),
            new(Species.Elephant, "Dumbo", new DateTime(2018, 3, 20), Gender.Male, FoodType.Vegetables)
        };

        _mockAnimalRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(animals);

        // Act
        var result = await _statisticsService.GetTotalAnimalCountAsync();

        // Assert
        Assert.Equal(3, result);
    }

    [Fact]
    public async Task GetAnimalsBySpecies_ReturnsCorrectCounts()
    {
        // Arrange
        var animals = new List<Animal>
        {
            new(Species.Lion, "Leo", new DateTime(2020, 1, 1), Gender.Male, FoodType.Meat),
            new(Species.Lion, "Leona", new DateTime(2021, 2, 2), Gender.Female, FoodType.Meat),
            new(Species.Tiger, "Tony", new DateTime(2019, 5, 15), Gender.Male, FoodType.Meat),
            new(Species.Elephant, "Dumbo", new DateTime(2018, 3, 20), Gender.Male, FoodType.Vegetables)
        };

        _mockAnimalRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(animals);

        // Act
        var result = await _statisticsService.GetAnimalsBySpeciesAsync();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(2, result[Species.Lion]);
        Assert.Equal(1, result[Species.Tiger]);
        Assert.Equal(1, result[Species.Elephant]);
    }

    [Fact]
    public async Task GetAnimalsByHealthStatus_ReturnsCorrectCounts()
    {
        // Arrange
        var animals = new List<Animal>
        {
            CreateAnimalWithHealthStatus(Species.Lion, "Leo", HealthStatus.Healthy),
            CreateAnimalWithHealthStatus(Species.Lion, "Leona", HealthStatus.Sick),
            CreateAnimalWithHealthStatus(Species.Tiger, "Tony", HealthStatus.Healthy),
            CreateAnimalWithHealthStatus(Species.Elephant, "Dumbo", HealthStatus.Recovering)
        };

        _mockAnimalRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(animals);

        // Act
        var result = await _statisticsService.GetAnimalsByHealthStatusAsync();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(2, result[HealthStatus.Healthy]);
        Assert.Equal(1, result[HealthStatus.Sick]);
        Assert.Equal(1, result[HealthStatus.Recovering]);
    }

    [Fact]
    public async Task GetAnimalsByEnclosure_ReturnsCorrectCounts()
    {
        // Arrange
        var enclosure1 = new Enclosure 
        { 
            Id = 1, 
            Name = "Lion Enclosure", 
            Type = EnclosureType.OpenAir,
            Size = 100.5,
            MaxCapacity = 5,
            CurrentOccupancy = 2,
            SpeciesType = Species.Lion
        };

        var enclosure2 = new Enclosure 
        { 
            Id = 2, 
            Name = "Tiger Enclosure", 
            Type = EnclosureType.Cage,
            Size = 80.0,
            MaxCapacity = 3,
            CurrentOccupancy = 1,
            SpeciesType = Species.Tiger
        };

        var animals = new List<Animal>
        {
            CreateAnimalWithEnclosure(Species.Lion, "Leo", enclosure1),
            CreateAnimalWithEnclosure(Species.Lion, "Leona", enclosure1),
            CreateAnimalWithEnclosure(Species.Tiger, "Tony", enclosure2),
            CreateAnimalWithEnclosure(Species.Elephant, "Dumbo", null) // No enclosure
        };

        _mockAnimalRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(animals);

        // Act
        var result = await _statisticsService.GetAnimalsByEnclosureAsync();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(2, result["Lion Enclosure"]);
        Assert.Equal(1, result["Tiger Enclosure"]);
        Assert.Equal(1, result["No Enclosure"]);
    }

    [Fact]
    public async Task GetAvailableEnclosuresCount_ReturnsCorrectCount()
    {
        // Arrange
        var enclosures = new List<Enclosure>
        {
            new Enclosure
            {
                Id = 1,
                Name = "Lion Enclosure",
                Type = EnclosureType.OpenAir,
                Size = 100.5,
                MaxCapacity = 5,
                CurrentOccupancy = 2,
                SpeciesType = Species.Lion
            },
            new Enclosure
            {
                Id = 2,
                Name = "Tiger Enclosure",
                Type = EnclosureType.Cage,
                Size = 80.0,
                MaxCapacity = 3,
                CurrentOccupancy = 3, // Full
                SpeciesType = Species.Tiger
            },
            new Enclosure
            {
                Id = 3,
                Name = "Elephant Enclosure",
                Type = EnclosureType.OpenAir,
                Size = 200.0,
                MaxCapacity = 4,
                CurrentOccupancy = 0, // Empty
                SpeciesType = Species.Elephant
            }
        };

        _mockEnclosureRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(enclosures);

        // Act
        var result = await _statisticsService.GetAvailableEnclosuresCountAsync();

        // Assert
        Assert.Equal(2, result); // Only Lion and Elephant enclosures have space
    }

    [Fact]
    public async Task GetOccupiedEnclosuresCount_ReturnsCorrectCount()
    {
        // Arrange
        var enclosures = new List<Enclosure>
        {
            new Enclosure
            {
                Id = 1,
                Name = "Lion Enclosure",
                Type = EnclosureType.OpenAir,
                Size = 100.5,
                MaxCapacity = 5,
                CurrentOccupancy = 2, // Occupied
                SpeciesType = Species.Lion
            },
            new Enclosure
            {
                Id = 2,
                Name = "Tiger Enclosure",
                Type = EnclosureType.Cage,
                Size = 80.0,
                MaxCapacity = 3,
                CurrentOccupancy = 3, // Occupied
                SpeciesType = Species.Tiger
            },
            new Enclosure
            {
                Id = 3,
                Name = "Elephant Enclosure",
                Type = EnclosureType.OpenAir,
                Size = 200.0,
                MaxCapacity = 4,
                CurrentOccupancy = 0, // Empty
                SpeciesType = Species.Elephant
            }
        };

        _mockEnclosureRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(enclosures);

        // Act
        var result = await _statisticsService.GetOccupiedEnclosuresCountAsync();

        // Assert
        Assert.Equal(2, result); // Lion and Tiger enclosures are occupied
    }

    [Fact]
    public async Task GetZooSummary_ReturnsFullSummary()
    {
        // Arrange
        var animals = new List<Animal>
        {
            CreateAnimalWithHealthStatus(Species.Lion, "Leo", HealthStatus.Healthy),
            CreateAnimalWithHealthStatus(Species.Lion, "Leona", HealthStatus.Sick),
            CreateAnimalWithHealthStatus(Species.Tiger, "Tony", HealthStatus.Healthy),
            CreateAnimalWithHealthStatus(Species.Elephant, "Dumbo", HealthStatus.Recovering)
        };

        var enclosures = new List<Enclosure>
        {
            new Enclosure
            {
                Id = 1,
                Name = "Lion Enclosure",
                Type = EnclosureType.OpenAir,
                Size = 100.5,
                MaxCapacity = 5,
                CurrentOccupancy = 2,
                SpeciesType = Species.Lion
            },
            new Enclosure
            {
                Id = 2,
                Name = "Tiger Enclosure",
                Type = EnclosureType.Cage,
                Size = 80.0,
                MaxCapacity = 3,
                CurrentOccupancy = 1,
                SpeciesType = Species.Tiger
            }
        };

        _mockAnimalRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(animals);

        _mockEnclosureRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(enclosures);

        // Act
        var result = await _statisticsService.GetZooSummaryAsync();

        // Assert
        Assert.Equal(4, result.TotalAnimals);
        Assert.Equal(2, result.TotalEnclosures);
        Assert.Equal(2, result.HealthyAnimals);
        Assert.Equal(1, result.SickAnimals);
        Assert.Equal(2, result.AvailableEnclosures);
        Assert.Equal(2, result.OccupiedEnclosures);
    }

    // Helper methods to create test animals
    private static Animal CreateAnimalWithHealthStatus(Species species, string name, HealthStatus status)
    {
        var animal = new Animal(
            species,
            name,
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        
        // Set health status using reflection
        var healthStatusProperty = typeof(Animal).GetProperty("HealthStatus");
        healthStatusProperty?.SetValue(animal, status);
        
        return animal;
    }
    
    private static Animal CreateAnimalWithEnclosure(Species species, string name, Enclosure? enclosure)
    {
        var animal = new Animal(
            species,
            name,
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        
        if (enclosure != null)
        {
            // Call MoveToEnclosure method
            animal.MoveToEnclosure(enclosure);
        }
        
        return animal;
    }
}
