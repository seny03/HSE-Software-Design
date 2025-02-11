using S4.HseCarShop.Models.Abstractions;

namespace S4.HseCarShop.Models;

/// <summary>
/// Покупатель
/// </summary>
internal sealed class Customer
{
    /// <summary>
    /// Имя покупателя
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Сила рук
    /// </summary>
    public uint HandStrength { get; }

    /// <summary>
    /// Сила ног
    /// </summary>
    public uint LegStrength { get; }

    /// <summary>
    /// Автомобиль
    /// </summary>
    public ICar? Car { get; set; }

    public Customer(string name, uint legStrength, uint handStrength)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        Name = name;
        LegStrength = legStrength;
        HandStrength = handStrength;
    }

    public override string ToString()
        => $"Имя: {Name}, Сила рук: {HandStrength}, Сила ног: {LegStrength}, Автомобиль: {(Car == null ? "Нет" : $"{{ {Car} }}")}";
}
