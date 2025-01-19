namespace HW1;

public class FactoryAF
{
    public List<Car> Cars { get; private set; }

    public List<Customer> Customers { get; private set; }

    public FactoryAF(List<Customer> customers)
    {
        Customers = customers;

        Cars = [];
    }

    internal void SaleCar()
    {
        foreach (var customer in Customers)
        {
            customer.Car ??= Cars.LastOrDefault();

            if (customer.Car == null)
                break;

            Cars.RemoveAt(Cars.Count - 1);
        }

        Customers = Customers.Where(customer => customer.Car != null).ToList();
        Cars.Clear();
    }

    internal void AddCar()
    {
        var car = new Car { Number = Cars.Count + 1 };
        Cars.Add(car);
    }
}