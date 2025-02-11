using S4.HseCarShop.Models;
using S4.HseCarShop.Services.Abstractions;

namespace S4.HseCarShop.Services;

/// <summary>
/// Хранилище покупателей
/// </summary>
internal class CustomersStorage : ICustomerStorage
{
    /// <summary>
    /// Список покупателей
    /// </summary>
    public List<Customer> Customers { get; }

    public CustomersStorage()
    {
        Customers = [];
    }

    /// <inheritdoc />
    public void AddCustomer(Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        Customers.Add(customer);
    }

    /// <inheritdoc />
    public void AddCustomers(IEnumerable<Customer> customers)
    {
        ArgumentNullException.ThrowIfNull(customers);

        Customers.AddRange(customers);
    }

    /// <inheritdoc />
    public IEnumerable<Customer> GetCustomers()
    {
        return Customers.Where(customer => customer.Car == null);
    }
}