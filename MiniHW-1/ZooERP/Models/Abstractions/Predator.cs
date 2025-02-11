namespace ZooERP.Models.Abstractions;

public abstract class Predator : Animal
{
    protected Predator(string name, int food, bool isHealthy, int number) : base(name, food, isHealthy, number)
    {
    }
}
