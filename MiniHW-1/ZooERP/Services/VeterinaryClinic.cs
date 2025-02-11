using ZooERP.Models.Abstractions;
using ZooERP.Services.Abstractions;

namespace ZooERP.Services;

public class VeterinaryClinic : IHealthValidator<Animal>
{
    public bool CheckHealth(Animal individual)
    {
        return individual.IsHealthy;
    }
}
