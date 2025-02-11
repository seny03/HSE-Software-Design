using S4.HseCarShop.Models.Abstractions;

namespace S4.HseCarShop.Models.HybridCar;

/// <summary>
/// Двигатель с педально-ручным приводом
/// </summary>
internal sealed class HybridEngine : EngineBase
{
    /// <summary>
    /// Тип ручек
    /// </summary>
    public GripsType GripsType { get; }

    /// <summary>
    /// Размер педалей
    /// </summary>
    public uint PedalsSize { get; }

    public HybridEngine(GripsType gripsType, uint pedalsSize)
        : base(EngineType.Hybrid)
    {
        GripsType = gripsType;
        PedalsSize = pedalsSize;
    }

    public override string ToString()
        => $"Тип: {Type}, Тип ручек: {GripsType}, Размер педалей: {PedalsSize}";
}
