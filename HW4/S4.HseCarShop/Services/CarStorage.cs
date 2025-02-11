using S4.HseCarShop.Models;
using S4.HseCarShop.Models.Abstractions;
using S4.HseCarShop.Services.Abstractions;

namespace S4.HseCarShop.Services;

/// <summary>
/// Хранилище автомобилей
/// </summary>
internal class CarStorage : ICarStorage
{
    /// <summary>
    /// Список автомобилей
    /// </summary>
    public LinkedList<ICar> Cars {  get; }

    public CarStorage()
    {
        Cars = new LinkedList<ICar>();
    }

    public void AddCar<TCar, TParams>(ICarFactory<TCar, TParams> carFactory, TParams carParams)
        where TCar : class, ICar
        where TParams : struct
    {
        var car = carFactory.CreateCar(carParams);
        Cars.AddLast(car);
    }

    public void AddCar(ICar car)
    {
        Cars.AddLast(car);
    }

    public void AddCars(IEnumerable<ICar> cars)
    {
        foreach (var car in cars)
            Cars.AddLast(car);
    }

    public ICar? GetCar(EngineType engineType)
    {
        var car = Cars.FirstOrDefault(car => car.Engine.Type == engineType);

        if (car != null)
            Cars.Remove(car);

        return car;
    }

    public ICar? GetCar(IEnumerable<CarType> carTypes)
    {
        var car = Cars.FirstOrDefault(car => carTypes.Contains(car.Type));

        if (car != null)
            Cars.Remove(car);

        return car;
    }
}
