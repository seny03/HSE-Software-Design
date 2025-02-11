using ZooERP.Models.Abstractions;

namespace ZooERP.Models.Animals;

public class Monkey : Herbo
{
    public Monkey(string name, int food, bool isHealthy, int number, uint kindness) : base(name, food, isHealthy, number, kindness)
    {
    }
}
