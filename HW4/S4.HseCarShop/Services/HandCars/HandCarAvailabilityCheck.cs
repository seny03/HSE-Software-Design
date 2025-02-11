using S4.HseCarShop.Models;
using S4.HseCarShop.Services.Abstractions;

namespace S4.HseCarShop.Services.HandCars;

internal class HandCarAvailabilityCheck : IAvailabilityCheck
{
    public CarType Type => CarType.Hand;

    public bool IsAvailable(CarAvailabilityParams args)
        => args.HandStrength > 5;
}
