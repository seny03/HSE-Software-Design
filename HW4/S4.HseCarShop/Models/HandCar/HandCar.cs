using S4.HseCarShop.Models.Abstractions;

namespace S4.HseCarShop.Models.HandCar;

/// <summary>
/// Ручной автомобиль
/// </summary>
internal sealed class HandCar : CarBase
{
    public HandCar(Guid number, HandEngine engine)
        : base(number, engine)
    {
    }
}
