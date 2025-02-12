using Bogus;
using S4.HseCarShop.Models;
using S4.HseCarShop.Models.HandCar;
using S4.HseCarShop.Models.HybridCar;
using S4.HseCarShop.Models.PedalCar;
using S4.HseCarShop.Services;
using S4.HseCarShop.Services.HandCars;
using S4.HseCarShop.Services.HybridCars;
using S4.HseCarShop.Services.PedalCars;

namespace S4.HseCarShop.Tests;

public class CarStorageTests
{
    private readonly Faker _faker;

    public CarStorageTests()
    {
        _faker = new Faker();
    }

    [Fact]
    [Trait("TestCategory", "CarStorage")]
    public void GetCar_HaveCarsInStorage_ReturnsSuitableCar()
    {
        // Arrange
        var engineType = _faker.Random.Enum<EngineType>(); // Use EngineType instead of CarType

        var carStorage = new CarStorage();

        var pedalCarFactory = new PedalCarFactory(new PedalEngineFactory());
        var handCarFactory = new HandCarFactory(new HandEngineFactory());
        var hybridCarFactory = new HybridCarFactory(new HybridEngineFactory());

        var pedalSize = (uint)_faker.Random.Int(1, 100);
        var pedalEngine = new PedalCarParams(pedalSize);

        carStorage.AddCar(pedalCarFactory, pedalEngine);
        carStorage.AddCar(handCarFactory, new HandCarParams());
        carStorage.AddCar(hybridCarFactory, new HybridCarParams());

        // Act
        var car = carStorage.GetCar(engineType);

        // Assert
        Assert.NotNull(car);
        Assert.Equal(engineType, car.Engine.Type);
    }


    [Theory]
    [InlineData(CarType.Hand)]
    [InlineData(CarType.Pedal)]
    [InlineData((CarType)42)]
    [Trait("TestCategory", "CarStorage")]
    internal void GetCar_EmptyStorage_ReturnsNull(CarType engineType)
    {
        // Arrange
        var carStorage = new CarStorage();


        // Act
        var car = carStorage.GetCar((EngineType)engineType);

        // Assert
        Assert.Null(car);
    }


    [Theory]
    [MemberData(nameof(GetCarTestData))]
    [Trait("TestCategory", "CarStorage")]
    internal void GetCar_EmptyStorage_ReturnsNull2(CarType engineType)
    {
        // Arrange
        var carStorage = new CarStorage();


        // Act
        var car = carStorage.GetCar((EngineType)engineType);

        // Assert
        Assert.Null(car);
    }

    //public static IEnumerable<object[]> GetCarTestData =>
    public static IEnumerable<object[]> GetCarTestData()
    {
        yield return new object[] { CarType.Pedal };
        yield return new object[] { CarType.Hand };
        yield return new object[] { (CarType)42 };
    }
}
