using S4.HseCarShop.Models.Abstractions;

namespace S4.HseCarShop.Services.Abstractions;

/// <summary>
/// Предоставляет функционал фабрики для создания автомобильных двигателей
/// </summary>
internal interface IEngineFactory<TEngine, TEngineParams>
    where TEngine : IEngine
    where TEngineParams : struct
{
    /// <summary>
    /// Создание двигателя с задаными параметрами
    /// </summary>
    TEngine CreateEngine(TEngineParams engineParams);
}
