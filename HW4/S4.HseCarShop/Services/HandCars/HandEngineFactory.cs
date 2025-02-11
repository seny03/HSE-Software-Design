using S4.HseCarShop.Models.HandCar;
using S4.HseCarShop.Services.Abstractions;

namespace S4.HseCarShop.Services.HandCars;

/// <summary>
/// Фабрика для создания ручных двигателей
/// </summary>
internal sealed class HandEngineFactory : IEngineFactory<HandEngine, HandEngineParams>
{
    /// <summary>
    /// Созданиие ручного двигателя
    /// </summary>
    /// <param name="engineParams">Параметры двигателя</param>
    /// <returns>Ручной двигатель</returns>
    public HandEngine CreateEngine(HandEngineParams engineParams)
    {
        return new HandEngine(engineParams.GripsType);
    }
}
