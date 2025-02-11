using S4.HseCarShop.Models.HybridCar;
using S4.HseCarShop.Models.PedalCar;
using S4.HseCarShop.Services.Abstractions;

namespace S4.HseCarShop.Services.PedalCars;

/// <summary>
/// Фабрика для создания педальных автомобилей
/// </summary>
internal sealed class PedalCarFactory : ICarFactory<PedalCar, PedalCarParams>
{
    private readonly IEngineFactory<PedalEngine, PedalEngineParams> _engineFactory;

    public PedalCarFactory(IEngineFactory<PedalEngine, PedalEngineParams> engineFactory)
    {
        ArgumentNullException.ThrowIfNull(engineFactory);

        _engineFactory = engineFactory;
    }

    /// <summary>
    /// Создание педального автомобиля
    /// </summary>
    /// <param name="carParams">Параметры автомобиля</param>
    /// <returns>Педальный автомобиль</returns>
    public PedalCar CreateCar(PedalCarParams carParams)
    {
        var engineParams = new PedalEngineParams { PedalSize = carParams.PedalSize };
        var engine = _engineFactory.CreateEngine(engineParams);
        return new PedalCar(number: Guid.NewGuid(), engine);
    }
}
