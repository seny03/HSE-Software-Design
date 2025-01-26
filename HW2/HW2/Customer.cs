namespace HW2;

public class Customer
{
    public string FIO { get; set; }
    public int ArmsPower { get; set; }
    public int LegsPower { get; set; }

    public Car? Car { get; set; }

    public Customer(string name, int legsPower, int armsPower)
    {
        FIO = name;
        ArmsPower = armsPower;
        LegsPower = legsPower;
    }

    public override string ToString()
    {
        return $"Имя: {FIO}, Номер машины: {Car?.Number ?? -1}";
    }
}