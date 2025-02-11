using S4.HseCarShop.Models;

namespace S4.HseCarShop.Services.Abstractions;

internal interface ICarAvailabilityService
{
    IEnumerable<CarType> GetAvailableCarTypes(CarAvailabilityParams args);
}
