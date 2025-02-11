using S4.HseCarShop.Models;
using S4.HseCarShop.Services.Abstractions;

namespace S4.HseCarShop.Services;

internal class CarAvailabilityService : ICarAvailabilityService
{
    private readonly IEnumerable<IAvailabilityCheck> _availabilityChecks;

    public CarAvailabilityService(IEnumerable<IAvailabilityCheck> availabilityChecks)
    {
        ArgumentNullException.ThrowIfNull(availabilityChecks);

        _availabilityChecks = availabilityChecks;
    }

    public IEnumerable<CarType> GetAvailableCarTypes(CarAvailabilityParams args)
    {
        return _availabilityChecks
            .Where(check => check.IsAvailable(args))
            .Select(check => check.Type);
    }
}
