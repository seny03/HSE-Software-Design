using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ZooWebApp.Domain.Common.Interfaces;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Domain.ValueObjects;
using ZooWebApp.Presentation.Controllers;
using ZooWebApp.Presentation.Models;

namespace ZooWebApp.Presentation.Tests.Controllers;

public class EnclosuresControllerTests
{
    private readonly Mock<IEnclosureRepository> _mockRepository;
    private readonly EnclosuresController _controller;

    public EnclosuresControllerTests()
    {
        _mockRepository = new Mock<IEnclosureRepository>();
        _controller = new EnclosuresController(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfEnclosures()
    {
        // Arrange
        var enclosures = new List<Enclosure>
        {
            new Enclosure
            {
                Id = 1,
                Name = "Lion Den",
                Type = EnclosureType.OpenAir,
                Size = 100.5,
                MaxCapacity = 5,
                CurrentOccupancy = 2,
                SpeciesType = Species.Lion
            },
            new Enclosure
            {
                Id = 2,
                Name = "Tiger Cage",
                Type = EnclosureType.Cage,
                Size = 80.0,
                MaxCapacity = 3,
                CurrentOccupancy = 1,
                SpeciesType = Species.Tiger
            }
        };

        _mockRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(enclosures);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedEnclosures = Assert.IsAssignableFrom<IEnumerable<Enclosure>>(okResult.Value);
        Assert.Equal(2, returnedEnclosures.Count());
        Assert.Contains(returnedEnclosures, e => e.Id == 1);
        Assert.Contains(returnedEnclosures, e => e.Id == 2);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WithEnclosure_WhenEnclosureExists()
    {
        // Arrange
        var enclosure = new Enclosure
        {
            Id = 1,
            Name = "Lion Den",
            Type = EnclosureType.OpenAir,
            Size = 100.5,
            MaxCapacity = 5,
            CurrentOccupancy = 2,
            SpeciesType = Species.Lion
        };

        _mockRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(enclosure);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedEnclosure = Assert.IsType<Enclosure>(okResult.Value);
        Assert.Equal(1, returnedEnclosure.Id);
        Assert.Equal("Lion Den", returnedEnclosure.Name);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenEnclosureDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Enclosure)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WithNewEnclosure()
    {
        // Arrange
        var createDto = new CreateEnclosureDto
        {
            Name = "Lion Den",
            Type = EnclosureType.OpenAir,
            Size = 100.5,
            MaxCapacity = 5,
            SpeciesType = Species.Lion
        };

        var createdEnclosure = new Enclosure
        {
            Id = 1,
            Name = "Lion Den",
            Type = EnclosureType.OpenAir,
            Size = 100.5,
            MaxCapacity = 5,
            CurrentOccupancy = 0,
            SpeciesType = Species.Lion
        };

        _mockRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<Enclosure> { createdEnclosure });

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(EnclosuresController.GetById), createdAtActionResult.ActionName);
        Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
        
        var returnedEnclosure = Assert.IsType<Enclosure>(createdAtActionResult.Value);
        Assert.Equal(1, returnedEnclosure.Id);
        Assert.Equal("Lion Den", returnedEnclosure.Name);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenEnclosureExistsAndUpdateIsSuccessful()
    {
        // Arrange
        var existingEnclosure = new Enclosure
        {
            Id = 1,
            Name = "Lion Den",
            Type = EnclosureType.OpenAir,
            Size = 100.5,
            MaxCapacity = 5,
            CurrentOccupancy = 0,
            SpeciesType = Species.Lion
        };

        var updateDto = new UpdateEnclosureDto
        {
            Name = "Lion Den Updated",
            Type = EnclosureType.OpenAir,
            Size = 120.5,
            CurrentOccupancy = 2,
            MaxCapacity = 8,
            SpeciesType = Species.Lion
        };

        _mockRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(existingEnclosure);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Enclosure>()), Times.Once);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenEnclosureDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Enclosure)null);

        var updateDto = new UpdateEnclosureDto
        {
            Name = "Lion Den Updated",
            Type = EnclosureType.OpenAir,
            Size = 120.5,
            CurrentOccupancy = 2,
            MaxCapacity = 8,
            SpeciesType = Species.Lion
        };

        // Act
        var result = await _controller.Update(999, updateDto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Enclosure>()), Times.Never);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenEnclosureExistsAndDeleteIsSuccessful()
    {
        // Arrange
        var existingEnclosure = new Enclosure
        {
            Id = 1,
            Name = "Lion Den",
            Type = EnclosureType.OpenAir,
            Size = 100.5,
            MaxCapacity = 5,
            CurrentOccupancy = 0,
            SpeciesType = Species.Lion
        };

        _mockRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(existingEnclosure);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenEnclosureDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Enclosure)null);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockRepository.Verify(repo => repo.DeleteAsync(999), Times.Never);
    }
}
