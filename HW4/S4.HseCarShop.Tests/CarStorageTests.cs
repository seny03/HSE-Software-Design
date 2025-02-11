using AutoFixture;
using AutoFixture.Xunit2;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using S2.HseCarShop.Models;
using S2.HseCarShop.Models.Abstractions;
using S2.HseCarShop.Services;
using S4.HseCarShop.Models;

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
        var engineType = _faker.Random.Enum<CarType>();

        var carStorage = new CarStorage();

        var pedalCarFactory = new PedalCarFactory();
        var handCarFactory = new HandCarFactory();

        var pedalSize = (uint)_faker.Random.Int(1, 100);
        var pedalEngine = new PedalCarParams(pedalSize);

        carStorage.AddCar(pedalCarFactory, pedalEngine);

        var emptyEngine = new HandEngineParams();

        carStorage.AddCar(handCarFactory, emptyEngine);

        // Act
        var car = carStorage.GetCar(engineType);

        // Assert
        Assert.NotNull(car);
        Assert.Equal(engineType, car.Engine.Type);
        
        //car.Should().NotBeNull();
        //car.Engine.Type.Should().Be(engineType);

        // Немного ужастей
        // persons.Should().ContainSingle(x => x.name == "Егор").Which.age.Should().BeGreaterThan(9);
        // age.Should().BeGreaterThan(9)
        // 10.May(2020).At(21, 20, 30)

        // А может давайте еще так хД
        // a = b; => Move().b.To().a;
    }

    [Theory]
    [InlineData(CarType.Hand)]
    [InlineData(CarType.Pedal)]
    [InlineData((CarType)42)]
    [Trait("TestCategory", "CarStorage")]
    public void GetCar_EmptyStorage_ReturnsNull(CarType engineType)
    {
        // Arrange

        // Act
        var carStorage = new CarStorage();
        var car = carStorage.GetCar(engineType);

        // Assert
        Assert.Null(car);
        
        //car.Should().BeNull();
    }


    [Theory]
    [MemberData(nameof(GetCarTestData))]
    [Trait("TestCategory", "CarStorage")]
    public void GetCar_EmptyStorage_ReturnsNull2(CarType engineType)
    {
        // Arrange

        // Act
        var carStorage = new CarStorage();
        var car = carStorage.GetCar(engineType);

        // Assert
        Assert.Null(car);

        //car.Should().BeNull();
    }

    //public static IEnumerable<object[]> GetCarTestData =>
    public static TheoryData<CarType> GetCarTestData =>
    [
        CarType.Pedal,
        CarType.Hand,
        (CarType)42,
    ];
}
