using Xunit;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Domain.Tests.ValueObjects;

public class EnclosureTests
{
    [Fact]
    public void Enclosure_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var name = "Lion's Den";
        var type = EnclosureType.Cage;
        var size = 100.0;
        var maxCapacity = 5;
        var currentOccupancy = 2;
        var speciesType = Species.Lion;

        // Act
        var enclosure = new Enclosure
        {
            Name = name,
            Type = type,
            Size = size,
            MaxCapacity = maxCapacity,
            CurrentOccupancy = currentOccupancy,
            SpeciesType = speciesType
        };

        // Assert
        Assert.Equal(name, enclosure.Name);
        Assert.Equal(type, enclosure.Type);
        Assert.Equal(size, enclosure.Size);
        Assert.Equal(maxCapacity, enclosure.MaxCapacity);
        Assert.Equal(currentOccupancy, enclosure.CurrentOccupancy);
        Assert.Equal(speciesType, enclosure.SpeciesType);
    }

    [Fact]
    public void Enclosure_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var enclosure1 = new Enclosure
        {
            Name = "Lion's Den",
            Type = EnclosureType.Cage,
            Size = 100.0,
            MaxCapacity = 5,
            CurrentOccupancy = 2,
            SpeciesType = Species.Lion
        };

        var enclosure2 = new Enclosure
        {
            Name = "Lion's Den",
            Type = EnclosureType.Cage,
            Size = 100.0,
            MaxCapacity = 5,
            CurrentOccupancy = 2,
            SpeciesType = Species.Lion
        };

        // Act & Assert
        Assert.Equal(enclosure1, enclosure2);
    }

    [Fact]
    public void Enclosure_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var enclosure1 = new Enclosure
        {
            Name = "Lion's Den",
            Type = EnclosureType.Cage,
            Size = 100.0,
            MaxCapacity = 5,
            CurrentOccupancy = 2,
            SpeciesType = Species.Lion
        };

        var enclosure2 = new Enclosure
        {
            Name = "Tiger's Den",
            Type = EnclosureType.Cage,
            Size = 100.0,
            MaxCapacity = 5,
            CurrentOccupancy = 2,
            SpeciesType = Species.Tiger
        };

        // Act & Assert
        Assert.NotEqual(enclosure1, enclosure2);
    }
}
