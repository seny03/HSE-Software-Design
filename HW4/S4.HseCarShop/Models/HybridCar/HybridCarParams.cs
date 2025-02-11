namespace S4.HseCarShop.Models.HybridCar;

/// <summary>
/// Параметры гибридного автомобиля
/// </summary>
/// <param name="GripsType">Тип ручек</param>
/// <param name="PedalSize">Размер педалей</param>
internal readonly record struct HybridCarParams(GripsType GripsType, uint PedalSize);
