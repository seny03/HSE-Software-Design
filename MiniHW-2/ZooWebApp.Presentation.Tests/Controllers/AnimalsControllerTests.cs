using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ZooWebApp.Application.Services;
using ZooWebApp.Domain.Entities;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Presentation.Controllers;
using ZooWebApp.Presentation.Models;

namespace ZooWebApp.Presentation.Tests.Controllers;

public class AnimalsControllerTests
{
    private readonly Mock<IAnimalService> _mockAnimalService;
    private readonly AnimalsController _controller;

    public AnimalsControllerTests()
    {
        _mockAnimalService = new Mock<IAnimalService>();
        _controller = new AnimalsController(_mockAnimalService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfAnimals()
    {
        // Arrange
        var animals = new List<Animal>
        {
            new(Species.Lion, "Leo", new DateTime(2020, 1, 1), Gender.Male, FoodType.Meat),
            new(Species.Tiger, "Tony", new DateTime(2019, 5, 15), Gender.Male, FoodType.Meat),
            new(Species.Elephant, "Dumbo", new DateTime(2018, 3, 20), Gender.Male, FoodType.Vegetables)
        };

        _mockAnimalService.Setup(service => service.GetAllAnimalsAsync())
            .ReturnsAsync(animals);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedAnimals = Assert.IsAssignableFrom<IEnumerable<Animal>>(okResult.Value);
        Assert.Equal(3, returnedAnimals.Count());
        Assert.Contains(returnedAnimals, a => a.Name == "Leo");
        Assert.Contains(returnedAnimals, a => a.Name == "Tony");
        Assert.Contains(returnedAnimals, a => a.Name == "Dumbo");
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WithAnimal_WhenAnimalExists()
    {
        // Arrange
        var animal = new Animal(Species.Lion, "Leo", new DateTime(2020, 1, 1), Gender.Male, FoodType.Meat);
        typeof(Animal).GetProperty("Id")?.SetValue(animal, 1);

        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(1))
            .ReturnsAsync(animal);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedAnimal = Assert.IsType<Animal>(okResult.Value);
        Assert.Equal(1, returnedAnimal.Id);
        Assert.Equal("Leo", returnedAnimal.Name);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenAnimalDoesNotExist()
    {
        // Arrange
        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(999))
            .ReturnsAsync((Animal)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WithNewAnimal()
    {
        // Arrange
        var createDto = new CreateAnimalDto
        {
            Name = "Leo",
            Species = Species.Lion,
            BirthDate = new DateTime(2020, 1, 1),
            Gender = Gender.Male,
            PreferredFood = FoodType.Meat
        };

        var createdAnimal = new Animal(Species.Lion, "Leo", new DateTime(2020, 1, 1), Gender.Male, FoodType.Meat);
        typeof(Animal).GetProperty("Id")?.SetValue(createdAnimal, 1);

        _mockAnimalService.Setup(service => service.AddAnimalAsync(It.IsAny<Animal>()))
            .Callback<Animal>(animal => typeof(Animal).GetProperty("Id")?.SetValue(animal, 1))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(AnimalsController.GetById), createdAtActionResult.ActionName);
        Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
        
        var returnedAnimal = Assert.IsType<Animal>(createdAtActionResult.Value);
        Assert.Equal(1, returnedAnimal.Id);
        Assert.Equal("Leo", returnedAnimal.Name);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenAnimalExistsAndUpdateIsSuccessful()
    {
        // Arrange
        var updateDto = new UpdateAnimalDto
        {
            Name = "Leo Updated",
            BirthDate = new DateTime(2020, 1, 1),
            Gender = Gender.Male,
            PreferredFood = FoodType.Meat,
            HealthStatus = HealthStatus.Healthy
        };

        var existingAnimal = new Animal(Species.Lion, "Leo", new DateTime(2020, 1, 1), Gender.Male, FoodType.Meat);
        typeof(Animal).GetProperty("Id")?.SetValue(existingAnimal, 1);

        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(1))
            .ReturnsAsync(existingAnimal);

        _mockAnimalService.Setup(service => service.UpdateAnimalAsync(It.IsAny<Animal>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockAnimalService.Verify(service => service.UpdateAnimalAsync(It.IsAny<Animal>()), Times.Once);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenAnimalDoesNotExist()
    {
        // Arrange
        var updateDto = new UpdateAnimalDto
        {
            Name = "Leo Updated",
            BirthDate = new DateTime(2020, 1, 1),
            Gender = Gender.Male,
            PreferredFood = FoodType.Meat,
            HealthStatus = HealthStatus.Healthy
        };

        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(999))
            .ReturnsAsync((Animal)null);

        // Act
        var result = await _controller.Update(999, updateDto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockAnimalService.Verify(service => service.UpdateAnimalAsync(It.IsAny<Animal>()), Times.Never);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenAnimalExistsAndDeleteIsSuccessful()
    {
        // Arrange
        var existingAnimal = new Animal(Species.Lion, "Leo", new DateTime(2020, 1, 1), Gender.Male, FoodType.Meat);
        typeof(Animal).GetProperty("Id")?.SetValue(existingAnimal, 1);

        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(1))
            .ReturnsAsync(existingAnimal);

        _mockAnimalService.Setup(service => service.DeleteAnimalAsync(1))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockAnimalService.Verify(service => service.DeleteAnimalAsync(1), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenAnimalDoesNotExist()
    {
        // Arrange
        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(999))
            .ReturnsAsync((Animal)null);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockAnimalService.Verify(service => service.DeleteAnimalAsync(999), Times.Never);
    }
}
