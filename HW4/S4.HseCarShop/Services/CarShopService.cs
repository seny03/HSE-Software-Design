using S4.HseCarShop.Models;
using S4.HseCarShop.Services.Abstractions;

namespace S4.HseCarShop.Services;

internal class CarShopService
{
    /// <summary>
    /// Сервис предоставляющий нам автомобили
    /// </summary>
    private readonly ICarProvider _carProvider;

    /// <summary>
    /// Сервис предоставляющий нам покупателей
    /// </summary>
    private readonly ICustomerProvider _customersProvider;

    /// <summary>
    /// Сервис для проверяющий какие автомобили подходят покупателю
    /// </summary>
    private readonly ICarAvailabilityService _availabilityService;

    public CarShopService(
        ICarProvider carProvider,
        ICustomerProvider customersProvider,
        ICarAvailabilityService availabilityService)
    {
        ArgumentNullException.ThrowIfNull(carProvider);
        ArgumentNullException.ThrowIfNull(customersProvider);
        ArgumentNullException.ThrowIfNull(availabilityService);

        _carProvider = carProvider;
        _customersProvider = customersProvider;
        _availabilityService = availabilityService;
        
    }

    public void SellCars()
    {
        var customers = _customersProvider.GetCustomers();

        foreach (var customer in customers)
        {
            if (customer.Car != null)
                continue;

            var availabilityParams = new CarAvailabilityParams
            {
                HandStrength = customer.HandStrength,
                LegStrength = customer.LegStrength,
            };
            var suitableEngineTypes = _availabilityService.GetAvailableCarTypes(availabilityParams);

            if (!suitableEngineTypes.Any())
                continue;

            var car = _carProvider.GetCar(suitableEngineTypes);

            if (car == null)
                continue;

            customer.Car = car;
        }
    }

    /// <summary>
    /// Определение типа двинателя который бы подошел пользователю по физическим параметрам
    /// </summary>
    /// <remarks>
    /// В условии это описано по другому, но я искренне полагаю, что прокидывать пользователя в класс двигателя это нарушение SRP.
    /// Мне наиболее логичным местом для проверки типа автомобиля подходящего для покупателя видится все же сам сервим для их продажи.
    /// </remarks>
    /// <param name="customer">Покупатель</param>
    /// <returns>Позвращает тип двигателя который подходит пользователю, если ни один не подходит, то null</returns>
    private static CarType? DetermineEngineType(Customer customer)
    {
        if (customer.LegStrength > 5)
            return CarType.Pedal;

        if (customer.HandStrength > 5)
            return CarType.Hand;

        return null;
    }
}
