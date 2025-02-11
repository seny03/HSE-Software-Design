using S4.HseCarShop.Models;
using S4.HseCarShop.Models.Abstractions;

namespace S4.HseCarShop.Services.Abstractions;

/// <summary>
/// Предоставляет функционал для получения автомобилей со склада
/// </summary>
internal interface ICarProvider
{
    /// <summary>
    /// Получение автомобиля с соответствующим типом двигателя
    /// </summary>
    /// <param name="carType">Тип автомобиля</param>
    /// <returns>Возвращает подходящую машину, а если таких машин не осталось - null</returns>
    ICar? GetCar(IEnumerable<CarType> carType);
}
