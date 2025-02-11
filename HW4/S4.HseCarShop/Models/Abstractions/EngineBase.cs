namespace S4.HseCarShop.Models.Abstractions;

/// <summary>
/// Базовая реализация двигателя
/// </summary>
internal abstract class EngineBase : IEngine
{
    /// <inheritdoc />
    public EngineType Type { get; }

    protected EngineBase(EngineType type)
    {
        Type = type;
    }

    public override string ToString()
        => $"Тип: {Type}";
}
