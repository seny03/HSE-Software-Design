using S4.HseCarShop.Models.Abstractions;

namespace S4.HseCarShop.Services.Abstractions;

/// <summary>
/// Предоставляет функционал для хранения автомобилей на складе
/// </summary>
internal interface ICarStorage : ICarProvider
{
    /// <summary>
    /// Добавляет автомобиль на склад
    /// </summary>
    /// <param name="car">Автомобиль</param>
    void AddCar(ICar car);

    /// <summary>
    /// Добавляет нескольких автомобилей на склад
    /// </summary>
    /// <param name="cars">Список автомобилей</param>
    void AddCars(IEnumerable<ICar> cars);

    /// <summary>
    /// Создание и добавление автомобиля с помощью переданной фабрики
    /// </summary>
    /// <typeparam name="TCar">Тип автомобиля</typeparam>
    /// <typeparam name="TParams">Параметры автомобиля</typeparam>
    /// <param name="carFactory">Обобщенная фабрика для создания автомобилей</param>
    /// <param name="carParams">Параметры для создания автомобиля</param>
    void AddCar<TCar, TParams>(ICarFactory<TCar, TParams> carFactory, TParams carParams)
        where TCar : class, ICar
        where TParams : struct;
}
