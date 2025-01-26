namespace HW2;

public class PedalEngine : IEngine
{
    public int Size { get; set; }

    public PedalEngine(int pedalSize)
    {
        Size = pedalSize;
    }
    public override string ToString()
    {
        return $"Размер педалей: {Size}";
    }

    public bool isCompatible(Customer customer)
    {
        return customer.LegsPower > 5;
    }

}