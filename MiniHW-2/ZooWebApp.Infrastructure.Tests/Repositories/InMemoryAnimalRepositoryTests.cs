using Xunit;
using ZooWebApp.Domain.Entities;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Infrastructure.Repositories;

namespace ZooWebApp.Infrastructure.Tests.Repositories;

public class InMemoryAnimalRepositoryTests
{
    [Fact]
    public async Task AddAsync_SetsIdAndAddsToCollection()
    {
        // Arrange
        var repository = new InMemoryAnimalRepository();
        var animal = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );

        // Act
        await repository.AddAsync(animal);
        var allAnimals = await repository.GetAllAsync();
        var retrievedAnimal = await repository.GetByIdAsync(1);

        // Assert
        Assert.Single(allAnimals);
        Assert.NotNull(retrievedAnimal);
        Assert.Equal(1, retrievedAnimal.Id);
        Assert.Equal("Leo", retrievedAnimal.Name);
    }

    [Fact]
    public async Task AddAsync_MultipleCalls_AssignsUniqueIds()
    {
        // Arrange
        var repository = new InMemoryAnimalRepository();
        var animal1 = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );

        var animal2 = new Animal(
            Species.Tiger,
            "Tony",
            new DateTime(2019, 5, 15),
            Gender.Male,
            FoodType.Meat
        );

        // Act
        await repository.AddAsync(animal1);
        await repository.AddAsync(animal2);
        var allAnimals = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, allAnimals.Count());
        Assert.Contains(allAnimals, a => a.Id == 1);
        Assert.Contains(allAnimals, a => a.Id == 2);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenAnimalDoesNotExist()
    {
        // Arrange
        var repository = new InMemoryAnimalRepository();

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingAnimal()
    {
        // Arrange
        var repository = new InMemoryAnimalRepository();
        var animal = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        await repository.AddAsync(animal);

        // Get the stored animal with its assigned ID
        var storedAnimal = await repository.GetByIdAsync(1);
        
        // Update the animal with the correct ID
        var updatedAnimal = new Animal(
            Species.Lion,
            "Leo Updated",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        // Set the ID to match the stored animal
        typeof(ZooWebApp.Domain.Common.EntityBase).GetProperty("Id")?.SetValue(updatedAnimal, 1);

        // Act
        await repository.UpdateAsync(updatedAnimal);
        var retrievedAnimal = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(retrievedAnimal);
        Assert.Equal("Leo Updated", retrievedAnimal.Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesAnimal()
    {
        // Arrange
        var repository = new InMemoryAnimalRepository();
        var animal = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        await repository.AddAsync(animal);

        // Act
        await repository.DeleteAsync(1);
        var allAnimals = await repository.GetAllAsync();
        var retrievedAnimal = await repository.GetByIdAsync(1);

        // Assert
        Assert.Empty(allAnimals);
        Assert.Null(retrievedAnimal);
    }
    
    [Fact]
    public async Task GetAllAsync_ReturnsEmptyCollection_WhenNoAnimalsExist()
    {
        // Arrange
        var repository = new InMemoryAnimalRepository();
        
        // Act
        var result = await repository.GetAllAsync();
        
        // Assert
        Assert.Empty(result);
        Assert.IsAssignableFrom<IEnumerable<Animal>>(result);
    }
    
    [Fact]
    public async Task UpdateAsync_DoesNothing_WhenAnimalDoesNotExist()
    {
        // Arrange
        var repository = new InMemoryAnimalRepository();
        var nonExistentAnimal = new Animal(
            Species.Lion,
            "Phantom",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        // Set a non-existent ID
        typeof(ZooWebApp.Domain.Common.EntityBase).GetProperty("Id")?.SetValue(nonExistentAnimal, 999);
        
        // Act - this should not throw
        await repository.UpdateAsync(nonExistentAnimal);
        var result = await repository.GetByIdAsync(999);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenAnimalDoesNotExist()
    {
        // Arrange
        var repository = new InMemoryAnimalRepository();
        
        // Act - this should not throw
        await repository.DeleteAsync(999);
        
        // Assert - no assertion needed, just verify it doesn't throw
    }
}
