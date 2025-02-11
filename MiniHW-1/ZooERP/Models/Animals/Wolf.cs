using ZooERP.Models.Abstractions;

namespace ZooERP.Models.Animals;

public class Wolf : Predator
{
    public Wolf(string name, int food, bool isHealthy, int number) : base(name, food, isHealthy, number)
    {
    }
}
