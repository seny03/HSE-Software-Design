using S4.HseCarShop.Models;

namespace S4.HseCarShop.Services.Abstractions;

internal interface IAvailabilityCheck
{
    CarType Type { get; }

    bool IsAvailable(CarAvailabilityParams args);
}
