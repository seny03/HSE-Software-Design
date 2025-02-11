namespace ZooERP.Models.Abstractions;

public abstract class Herbo : Animal
{
    public uint Kindness
    {
        get => _kindness;
        private init
        {
            if (0 <= value && value <= 10)
            {
                _kindness = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Kindess must be in range [0; 10], but {value} was given.");
            }
        }
    }

    private readonly uint _kindness;
    protected Herbo(string name, int food, bool isHealthy, int number, uint kindness) : base(name, food, isHealthy, number)
    {
        Kindness = kindness;
    }
}
