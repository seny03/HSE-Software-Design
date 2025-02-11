using S4.HseCarShop.Models.Abstractions;

namespace S4.HseCarShop.Models.PedalCar;

/// <summary>
/// Педальный автомобиль
/// </summary>
internal sealed class PedalCar : CarBase
{
    public PedalCar(Guid number, PedalEngine engine)
        : base(number, engine)
    {
    }
}
