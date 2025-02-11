namespace ZooERP.Models.Abstractions;

public abstract class Thing : IInventory
{
    public int Number { get; init; }

    public Thing(int number)
    {
        Number = number;
    }
}
