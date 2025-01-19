namespace HW1;

public class Car
{
    private static readonly Random _random = new();

    public required int Number { get; set; }

    public Engine Engine { get; }

    public Car()
    {
        Engine = new Engine { Size = _random.Next(1, 10) };
    }

    public override string ToString()
    {
        return $"Номер: {Number}, {Engine}";
    }
}