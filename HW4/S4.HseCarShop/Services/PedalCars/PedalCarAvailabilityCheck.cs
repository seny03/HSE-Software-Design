using S4.HseCarShop.Models;
using S4.HseCarShop.Services.Abstractions;

namespace S4.HseCarShop.Services.PedalCars;

internal class PedalCarAvailabilityCheck : IAvailabilityCheck
{
    public CarType Type => CarType.Pedal;

    public bool IsAvailable(CarAvailabilityParams args)
        => args.LegStrength > 5;
}
