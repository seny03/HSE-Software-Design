using S4.HseCarShop.Models;
using S4.HseCarShop.Services.Abstractions;

namespace S4.HseCarShop.Services.HybridCars;

internal class HybridCarAvailabilityCheck : IAvailabilityCheck
{
    public CarType Type => CarType.Hybrid;

    public bool IsAvailable(CarAvailabilityParams args)
        => args.HandStrength < 5 && args.LegStrength < 5;
}