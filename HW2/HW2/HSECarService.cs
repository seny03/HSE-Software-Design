namespace HW2;
public class HseCarService
{
    private readonly ICarProvider _carProvider;

    private readonly ICustomersProvider _customersProvider;

    public HseCarService(ICarProvider carProvider, ICustomersProvider customersProvider)
    {
        ArgumentNullException.ThrowIfNull(carProvider, nameof(carProvider));
        ArgumentNullException.ThrowIfNull(customersProvider, nameof(customersProvider));

        _carProvider = carProvider;
        _customersProvider = customersProvider;
    }

    public void SellCars()
    {
        var customers = _customersProvider.GetCustomers();

        foreach (Customer customer in customers)
        {
            if (customer.Car != null)
                continue;

            var car = _carProvider.GetCar(customer);

            if (car == null)
                continue;

            customer.Car = car;
        }
    }
}