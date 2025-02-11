using S4.HseCarShop.Models.PedalCar;
using S4.HseCarShop.Services.Abstractions;

namespace S4.HseCarShop.Services.PedalCars;

/// <summary>
/// Фабрика для создания двигателей с педальным приводом
/// </summary>
internal sealed class PedalEngineFactory : IEngineFactory<PedalEngine, PedalEngineParams>
{
    /// <summary>
    /// Создание двигателя с педальным приводом
    /// </summary>
    /// <param name="engineParams">Параметры двигателя</param>
    /// <returns>Педальный двигатель</returns>
    public PedalEngine CreateEngine(PedalEngineParams engineParams)
    {
        return new PedalEngine(engineParams.PedalSize);
    }
}

