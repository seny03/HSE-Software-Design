using Moq;
using Xunit;
using ZooWebApp.Application.Services;
using ZooWebApp.Domain.Common;
using ZooWebApp.Domain.Common.Interfaces;
using ZooWebApp.Domain.Entities;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Application.Tests.Services;

public class AnimalServiceTests
{
    private readonly Mock<IAnimalRepository> _mockAnimalRepository;
    private readonly AnimalService _animalService;

    public AnimalServiceTests()
    {
        _mockAnimalRepository = new Mock<IAnimalRepository>();
        _animalService = new AnimalService(_mockAnimalRepository.Object);
    }

    [Fact]
    public async Task GetAnimalByIdAsync_ReturnsAnimal_WhenAnimalExists()
    {
        // Arrange
        var expectedAnimal = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        typeof(EntityBase).GetProperty("Id")?.SetValue(expectedAnimal, 1);

        _mockAnimalRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(expectedAnimal);

        // Act
        var result = await _animalService.GetAnimalByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedAnimal.Id, result.Id);
        Assert.Equal(expectedAnimal.Name, result.Name);
        Assert.Equal(expectedAnimal.Species, result.Species);
    }

    [Fact]
    public async Task GetAnimalByIdAsync_ReturnsNull_WhenAnimalDoesNotExist()
    {
        // Arrange
        _mockAnimalRepository.Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Animal)null!);

        // Act
        var result = await _animalService.GetAnimalByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAnimalsAsync_ReturnsAllAnimals()
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
        var result = await _animalService.GetAllAnimalsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task AddAnimalAsync_AddsAnimal()
    {
        // Arrange
        var animal = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );

        // Act
        await _animalService.AddAnimalAsync(animal);

        // Assert
        _mockAnimalRepository.Verify(repo => repo.AddAsync(animal), Times.Once);
    }

    [Fact]
    public async Task UpdateAnimalAsync_UpdatesExistingAnimal()
    {
        // Arrange
        var existingAnimal = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        typeof(EntityBase).GetProperty("Id")?.SetValue(existingAnimal, 1);

        var updatedAnimal = new Animal(
            Species.Lion,
            "Leo Updated",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        typeof(EntityBase).GetProperty("Id")?.SetValue(updatedAnimal, 1);

        _mockAnimalRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(existingAnimal);

        // Act
        await _animalService.UpdateAnimalAsync(updatedAnimal);

        // Assert
        _mockAnimalRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Animal>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAnimalAsync_DeletesAnimal()
    {
        // Arrange
        const int animalId = 1;

        // Act
        await _animalService.DeleteAnimalAsync(animalId);

        // Assert
        _mockAnimalRepository.Verify(repo => repo.DeleteAsync(animalId), Times.Once);
    }
}
