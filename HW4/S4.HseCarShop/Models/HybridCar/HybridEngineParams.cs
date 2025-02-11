namespace S4.HseCarShop.Models.HybridCar;

/// <summary>
/// Параметры гибридного двигателя
/// </summary>
/// <param name="GripsType">Тип ручек</param>
/// <param name="PedalSize">Размер педалей</param>
internal readonly record struct HybridEngineParams(GripsType GripsType, uint PedalSize);
