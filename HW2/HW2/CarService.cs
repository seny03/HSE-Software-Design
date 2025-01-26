namespace HW2;

public class CarService : ICarProvider
{
    private readonly LinkedList<Car> _cars = new();
    private static int _curId = 0;

    public void AddCar<TParams>(ICarFactory<TParams> carFactory, TParams carParams)
        where TParams : EngineParamsBase
    {
        var car = carFactory.CreateCar(carParams, _curId++);
        _cars.AddLast(car);
    }

    public Car? GetCar(Customer customer)
    {
        Car? firstCompatibleCar = null;

        foreach (Car car in _cars)
        {
            if (car.isCompatible(customer))
            {
                firstCompatibleCar = car;
                break;
            }
        }

        if (firstCompatibleCar != null)
            _cars.Remove(firstCompatibleCar);

        return firstCompatibleCar;
    }
}