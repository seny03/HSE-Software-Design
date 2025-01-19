namespace HW1;

public class Customer
{
    public required string FIO { get; set; }

    public Car? Car { get; set; }

    public override string ToString()
    {
        return $"Имя: {FIO}, Номер машины: {Car?.Number ?? -1}";
    }
}