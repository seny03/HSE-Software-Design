using S4.HseCarShop.Models.Abstractions;

namespace S4.HseCarShop.Models.HandCar;

/// <summary>
/// Двигатель с ручным приводом
/// </summary>
internal sealed class HandEngine : EngineBase
{
    /// <summary>
    /// Тип ручек
    /// </summary>
    public GripsType GripsType { get; }

    public HandEngine(GripsType gripsType)
        : base(EngineType.Hand)
    {
        GripsType = gripsType;
    }

    public override string ToString()
        => $"Тип: {Type}, Тип ручек: {GripsType}";
}