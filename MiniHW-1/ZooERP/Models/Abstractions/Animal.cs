namespace ZooERP.Models.Abstractions;

public abstract class Animal : IAlive, IInventory
{
    public string Name { get; set; }
    public int Food { get; set; }
    public bool IsHealthy { get; set; }
    public int Number { get; init; }

    protected Animal(string name, int food, bool isHealthy, int number)
    {
        Name = name;
        Food = food;
        IsHealthy = isHealthy;
        Number = number;
    }
}
