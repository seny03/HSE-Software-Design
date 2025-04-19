using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ZooWebApp.Application.Services;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Presentation.Controllers;

namespace ZooWebApp.Presentation.Tests.Controllers;

public class ZooStatisticsControllerTests
{
    private readonly Mock<IZooStatisticsService> _mockStatisticsService;
    private readonly ZooStatisticsController _controller;

    public ZooStatisticsControllerTests()
    {
        _mockStatisticsService = new Mock<IZooStatisticsService>();
        _controller = new ZooStatisticsController(_mockStatisticsService.Object);
    }

    [Fact]
    public async Task GetTotalAnimalCount_ReturnsOkResult_WithCount()
    {
        // Arrange
        const int expectedCount = 10;
        _mockStatisticsService.Setup(service => service.GetTotalAnimalCountAsync())
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _controller.GetTotalAnimalCount();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var count = Assert.IsType<int>(okResult.Value);
        Assert.Equal(expectedCount, count);
    }

    [Fact]
    public async Task GetAnimalsBySpecies_ReturnsOkResult_WithDictionary()
    {
        // Arrange
        var animalsBySpecies = new Dictionary<Species, int>
        {
            { Species.Lion, 2 },
            { Species.Tiger, 1 },
            { Species.Elephant, 3 }
        };

        _mockStatisticsService.Setup(service => service.GetAnimalsBySpeciesAsync())
            .ReturnsAsync(animalsBySpecies);

        // Act
        var result = await _controller.GetAnimalsBySpecies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dictionary = Assert.IsType<Dictionary<Species, int>>(okResult.Value);
        Assert.Equal(3, dictionary.Count);
        Assert.Equal(2, dictionary[Species.Lion]);
        Assert.Equal(1, dictionary[Species.Tiger]);
        Assert.Equal(3, dictionary[Species.Elephant]);
    }

    [Fact]
    public async Task GetAnimalsByHealthStatus_ReturnsOkResult_WithDictionary()
    {
        // Arrange
        var animalsByHealthStatus = new Dictionary<HealthStatus, int>
        {
            { HealthStatus.Healthy, 5 },
            { HealthStatus.Sick, 2 },
            { HealthStatus.Recovering, 3 }
        };

        _mockStatisticsService.Setup(service => service.GetAnimalsByHealthStatusAsync())
            .ReturnsAsync(animalsByHealthStatus);

        // Act
        var result = await _controller.GetAnimalsByHealthStatus();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dictionary = Assert.IsType<Dictionary<HealthStatus, int>>(okResult.Value);
        Assert.Equal(3, dictionary.Count);
        Assert.Equal(5, dictionary[HealthStatus.Healthy]);
        Assert.Equal(2, dictionary[HealthStatus.Sick]);
        Assert.Equal(3, dictionary[HealthStatus.Recovering]);
    }

    [Fact]
    public async Task GetAnimalsByEnclosure_ReturnsOkResult_WithDictionary()
    {
        // Arrange
        var animalsByEnclosure = new Dictionary<string, int>
        {
            { "Lion Enclosure", 2 },
            { "Tiger Enclosure", 1 },
            { "No Enclosure", 3 }
        };

        _mockStatisticsService.Setup(service => service.GetAnimalsByEnclosureAsync())
            .ReturnsAsync(animalsByEnclosure);

        // Act
        var result = await _controller.GetAnimalsByEnclosure();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dictionary = Assert.IsType<Dictionary<string, int>>(okResult.Value);
        Assert.Equal(3, dictionary.Count);
        Assert.Equal(2, dictionary["Lion Enclosure"]);
        Assert.Equal(1, dictionary["Tiger Enclosure"]);
        Assert.Equal(3, dictionary["No Enclosure"]);
    }

    [Fact]
    public async Task GetAvailableEnclosures_ReturnsOkResult_WithCount()
    {
        // Arrange
        const int expectedCount = 3;
        _mockStatisticsService.Setup(service => service.GetAvailableEnclosuresCountAsync())
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _controller.GetAvailableEnclosures();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var count = Assert.IsType<int>(okResult.Value);
        Assert.Equal(expectedCount, count);
    }

    [Fact]
    public async Task GetOccupiedEnclosures_ReturnsOkResult_WithCount()
    {
        // Arrange
        const int expectedCount = 4;
        _mockStatisticsService.Setup(service => service.GetOccupiedEnclosuresCountAsync())
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _controller.GetOccupiedEnclosures();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var count = Assert.IsType<int>(okResult.Value);
        Assert.Equal(expectedCount, count);
    }

    [Fact]
    public async Task GetSummary_ReturnsOkResult_WithZooSummary()
    {
        // Arrange
        var zooSummary = new ZooSummary
        {
            TotalAnimals = 10,
            TotalEnclosures = 5,
            HealthyAnimals = 7,
            SickAnimals = 1,
            AvailableEnclosures = 2,
            OccupiedEnclosures = 3
        };

        _mockStatisticsService.Setup(service => service.GetZooSummaryAsync())
            .ReturnsAsync(zooSummary);

        // Act
        var result = await _controller.GetSummary();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var summary = Assert.IsType<ZooSummary>(okResult.Value);
        Assert.Equal(10, summary.TotalAnimals);
        Assert.Equal(5, summary.TotalEnclosures);
        Assert.Equal(7, summary.HealthyAnimals);
        Assert.Equal(1, summary.SickAnimals);
        Assert.Equal(2, summary.AvailableEnclosures);
        Assert.Equal(3, summary.OccupiedEnclosures);
    }
}
