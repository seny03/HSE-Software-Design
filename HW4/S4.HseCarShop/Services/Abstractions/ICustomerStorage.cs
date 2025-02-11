using S4.HseCarShop.Models;

namespace S4.HseCarShop.Services.Abstractions;

/// <summary>
/// Предоставляет функционал для хранения очереди покупателей машин
/// </summary>
internal interface ICustomerStorage : ICustomerProvider
{
    /// <summary>
    /// Добавляет покупателя в очередь за машиной
    /// </summary>
    /// <param name="customer">Покупатель</param>
    void AddCustomer(Customer customer);

    /// <summary>
    /// Добавляет нескольких покупателей в очередь за машиной
    /// </summary>
    /// <param name="customers">Список покупателей</param>
    void AddCustomers(IEnumerable<Customer> customers);
}
