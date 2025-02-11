using S4.HseCarShop.Models.Abstractions;

namespace S4.HseCarShop.Models.PedalCar;

/// <summary>
/// Двигатель с педальным приводом
/// </summary>
internal sealed class PedalEngine : EngineBase
{
    /// <summary>
    /// Размер педалей
    /// </summary>
    public uint PedalsSize { get; }

    public PedalEngine(uint pedalsSize)
        : base(EngineType.Pedal)
    {
        PedalsSize = pedalsSize;
    }

    public override string ToString()
        => $"Тип: {Type}, Размер педалей: {PedalsSize}";
}