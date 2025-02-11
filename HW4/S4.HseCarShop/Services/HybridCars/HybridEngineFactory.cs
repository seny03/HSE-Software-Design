using S4.HseCarShop.Models.HybridCar;
using S4.HseCarShop.Services.Abstractions;

namespace S4.HseCarShop.Services.HybridCars;

/// <summary>
/// Фабрика для создания двигателей с педально-ручным приводом.
/// </summary>
internal sealed class HybridEngineFactory : IEngineFactory<HybridEngine, HybridEngineParams>
{
    /// <summary>
    /// Создание двигателя с педально-ручным приводом
    /// </summary>
    /// <param name="engineParams">Параметры двигателя</param>
    /// <returns>Гибридный двигатель</returns>
    public HybridEngine CreateEngine(HybridEngineParams engineParams)
    {
        return new HybridEngine(engineParams.GripsType, engineParams.PedalSize);
    }
}