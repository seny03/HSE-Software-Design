using ZooERP.Models.Abstractions;

namespace ZooERP.Models.Animals;

public class Rabbit : Herbo
{
    public Rabbit(string name, int food, bool isHealthy, int number, uint kindness) : base(name, food, isHealthy, number, kindness)
    {
    }
}
