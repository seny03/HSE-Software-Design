using S4.HseCarShop.Models.HybridCar;
using S4.HseCarShop.Services.Abstractions;

namespace S4.HseCarShop.Services.HybridCars;

/// <summary>
/// Фабрика для создания педальных автомобилей
/// </summary>
internal sealed class HybridCarFactory : ICarFactory<HybridCar, HybridCarParams>
{
    private readonly IEngineFactory<HybridEngine, HybridEngineParams> _engineFactory;

    public HybridCarFactory(IEngineFactory<HybridEngine, HybridEngineParams> engineFactory)
    {
        ArgumentNullException.ThrowIfNull(engineFactory);

        _engineFactory = engineFactory;
    }

    /// <summary>
    /// Создание гибридного автомобиля
    /// </summary>
    /// <param name="carParams">Параметры автомобиля</param>
    /// <returns>Гибридный автомобиль</returns>
    public HybridCar CreateCar(HybridCarParams carParams)
    {
        var engineParams = new HybridEngineParams
        {
            GripsType = carParams.GripsType,
            PedalSize = carParams.PedalSize,
        };

        var engine = _engineFactory.CreateEngine(engineParams);
        return new HybridCar(number: Guid.NewGuid(), engine);
    }
}