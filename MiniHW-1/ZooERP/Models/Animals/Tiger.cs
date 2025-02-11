using ZooERP.Models.Abstractions;

namespace ZooERP.Models.Animals;

public class Tiger : Predator
{
    public Tiger(string name, int food, bool isHealthy, int number) : base(name, food, isHealthy, number)
    {
    }
}
