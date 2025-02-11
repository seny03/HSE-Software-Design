using S4.HseCarShop.Models.Abstractions;

namespace S4.HseCarShop.Services.Abstractions;

/// <summary>
/// Обобщенный интерфейс фабрики для создания автомобилей
/// </summary>
internal interface ICarFactory<TCar, TCarParams>
    where TCar : ICar
    where TCarParams : struct
{
    /// <summary>
    /// Метод для конструирования автомобилей
    /// </summary>
    TCar CreateCar(TCarParams carParams);
}
