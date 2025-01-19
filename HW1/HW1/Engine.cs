namespace HW1;

public class Engine
{
    public required int Size { get; set; }
    public override string ToString()
    {
        return $"Размер педалей: {Size}";
    }
}