using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ZooWebApp.Application.Services;
using ZooWebApp.Domain.Entities;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Presentation.Controllers;
using ZooWebApp.Presentation.Models;

namespace ZooWebApp.Presentation.Tests.Controllers;

public class FeedingSchedulesControllerTests
{
    private readonly Mock<IFeedingScheduleService> _mockFeedingService;
    private readonly Mock<IAnimalService> _mockAnimalService;
    private readonly FeedingSchedulesController _controller;

    public FeedingSchedulesControllerTests()
    {
        _mockFeedingService = new Mock<IFeedingScheduleService>();
        _mockAnimalService = new Mock<IAnimalService>();
        _controller = new FeedingSchedulesController(_mockFeedingService.Object, _mockAnimalService.Object);
    }

    [Fact]
    public async Task Create_ReturnsOkResult_WhenCreateIsSuccessful()
    {
        // Arrange
        var createDto = new CreateFeedingScheduleDto
        {
            AnimalId = 1,
            FeedingTime = TimeSpan.FromHours(10),
            FoodType = FoodType.Meat,
            Quantity = 2.5,
            FeedingDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday }
        };

        var animal = new Animal(Species.Lion, "Leo", new DateTime(2020, 1, 1), Gender.Male, FoodType.Meat);
        typeof(Animal).GetProperty("Id")?.SetValue(animal, 1);

        var result = new OperationResult { IsSuccess = true };

        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(1))
            .ReturnsAsync(animal);

        _mockFeedingService.Setup(service => service.CreateFeedingScheduleAsync(
                1, 
                TimeSpan.FromHours(10), 
                FoodType.Meat, 
                2.5, 
                It.IsAny<ICollection<DayOfWeek>>()))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.Create(createDto);

        // Assert
        var okResult = Assert.IsType<OkResult>(actionResult);
        _mockFeedingService.Verify(service => service.CreateFeedingScheduleAsync(
            1, TimeSpan.FromHours(10), FoodType.Meat, 2.5, It.IsAny<ICollection<DayOfWeek>>()), 
            Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenAnimalNotFound()
    {
        // Arrange
        var createDto = new CreateFeedingScheduleDto
        {
            AnimalId = 999,
            FeedingTime = TimeSpan.FromHours(10),
            FoodType = FoodType.Meat,
            Quantity = 2.5,
            FeedingDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday }
        };

        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(999))
            .ReturnsAsync((Animal)null);

        // Act
        var actionResult = await _controller.Create(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Animal with ID 999 not found", badRequestResult.Value);
        
        _mockFeedingService.Verify(service => service.CreateFeedingScheduleAsync(
            It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<FoodType>(), 
            It.IsAny<double>(), It.IsAny<ICollection<DayOfWeek>>()), 
            Times.Never);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenOperationFails()
    {
        // Arrange
        var createDto = new CreateFeedingScheduleDto
        {
            AnimalId = 1,
            FeedingTime = TimeSpan.FromHours(10),
            FoodType = FoodType.Meat,
            Quantity = 2.5,
            FeedingDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday }
        };

        var animal = new Animal(Species.Lion, "Leo", new DateTime(2020, 1, 1), Gender.Male, FoodType.Meat);
        typeof(Animal).GetProperty("Id")?.SetValue(animal, 1);

        var result = new OperationResult 
        { 
            IsSuccess = false,
            ErrorMessage = "Invalid food type for this animal" 
        };

        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(1))
            .ReturnsAsync(animal);

        _mockFeedingService.Setup(service => service.CreateFeedingScheduleAsync(
                1, 
                TimeSpan.FromHours(10), 
                FoodType.Meat, 
                2.5, 
                It.IsAny<ICollection<DayOfWeek>>()))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.Create(createDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Invalid food type for this animal", badRequestResult.Value);
    }

    [Fact]
    public async Task Update_ReturnsOkResult_WhenUpdateIsSuccessful()
    {
        // Arrange
        var updateDto = new UpdateFeedingScheduleDto
        {
            FeedingTime = TimeSpan.FromHours(12),
            FoodType = FoodType.Meat,
            Quantity = 3.0,
            FeedingDays = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday }
        };

        var animal = new Animal(Species.Lion, "Leo", new DateTime(2020, 1, 1), Gender.Male, FoodType.Meat);
        typeof(Animal).GetProperty("Id")?.SetValue(animal, 1);

        var result = new OperationResult { IsSuccess = true };

        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(1))
            .ReturnsAsync(animal);

        _mockFeedingService.Setup(service => service.UpdateFeedingScheduleAsync(
                1, 
                TimeSpan.FromHours(12), 
                FoodType.Meat, 
                3.0, 
                It.IsAny<ICollection<DayOfWeek>>()))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.Update(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkResult>(actionResult);
        _mockFeedingService.Verify(service => service.UpdateFeedingScheduleAsync(
            1, TimeSpan.FromHours(12), FoodType.Meat, 3.0, It.IsAny<ICollection<DayOfWeek>>()), 
            Times.Once);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenAnimalNotFound()
    {
        // Arrange
        var updateDto = new UpdateFeedingScheduleDto
        {
            FeedingTime = TimeSpan.FromHours(12),
            FoodType = FoodType.Meat,
            Quantity = 3.0,
            FeedingDays = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday }
        };

        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(999))
            .ReturnsAsync((Animal)null);

        // Act
        var actionResult = await _controller.Update(999, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Animal with ID 999 not found", badRequestResult.Value);
        
        _mockFeedingService.Verify(service => service.UpdateFeedingScheduleAsync(
            It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<FoodType>(), 
            It.IsAny<double>(), It.IsAny<ICollection<DayOfWeek>>()), 
            Times.Never);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenOperationFails()
    {
        // Arrange
        var updateDto = new UpdateFeedingScheduleDto
        {
            FeedingTime = TimeSpan.FromHours(12),
            FoodType = FoodType.Meat,
            Quantity = 3.0,
            FeedingDays = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday }
        };

        var animal = new Animal(Species.Lion, "Leo", new DateTime(2020, 1, 1), Gender.Male, FoodType.Meat);
        typeof(Animal).GetProperty("Id")?.SetValue(animal, 1);

        var result = new OperationResult 
        { 
            IsSuccess = false, 
            ErrorMessage = "Animal does not have a feeding schedule" 
        };

        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(1))
            .ReturnsAsync(animal);

        _mockFeedingService.Setup(service => service.UpdateFeedingScheduleAsync(
                1, 
                TimeSpan.FromHours(12), 
                FoodType.Meat, 
                3.0, 
                It.IsAny<ICollection<DayOfWeek>>()))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.Update(1, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Animal does not have a feeding schedule", badRequestResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsOkResult_WhenDeleteIsSuccessful()
    {
        // Arrange
        var animal = new Animal(Species.Lion, "Leo", new DateTime(2020, 1, 1), Gender.Male, FoodType.Meat);
        typeof(Animal).GetProperty("Id")?.SetValue(animal, 1);

        var result = new OperationResult { IsSuccess = true };

        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(1))
            .ReturnsAsync(animal);

        _mockFeedingService.Setup(service => service.RemoveFeedingScheduleAsync(1))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.Delete(1);

        // Assert
        var okResult = Assert.IsType<OkResult>(actionResult);
        _mockFeedingService.Verify(service => service.RemoveFeedingScheduleAsync(1), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenAnimalNotFound()
    {
        // Arrange
        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(999))
            .ReturnsAsync((Animal)null);

        // Act
        var actionResult = await _controller.Delete(999);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Animal with ID 999 not found", badRequestResult.Value);
        
        _mockFeedingService.Verify(service => service.RemoveFeedingScheduleAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenOperationFails()
    {
        // Arrange
        var animal = new Animal(Species.Lion, "Leo", new DateTime(2020, 1, 1), Gender.Male, FoodType.Meat);
        typeof(Animal).GetProperty("Id")?.SetValue(animal, 1);

        var result = new OperationResult 
        { 
            IsSuccess = false, 
            ErrorMessage = "Animal does not have a feeding schedule" 
        };

        _mockAnimalService.Setup(service => service.GetAnimalByIdAsync(1))
            .ReturnsAsync(animal);

        _mockFeedingService.Setup(service => service.RemoveFeedingScheduleAsync(1))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.Delete(1);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Animal does not have a feeding schedule", badRequestResult.Value);
    }
}
