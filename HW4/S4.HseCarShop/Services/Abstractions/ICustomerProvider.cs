using S4.HseCarShop.Models;

namespace S4.HseCarShop.Services.Abstractions;

/// <summary>
/// Предоставляет функционал получения списка покупателей машин
/// </summary>
internal interface ICustomerProvider
{
    /// <summary>
    /// Возвращает покупателей в очереди за машиной
    /// </summary>
    IEnumerable<Customer> GetCustomers();
}
