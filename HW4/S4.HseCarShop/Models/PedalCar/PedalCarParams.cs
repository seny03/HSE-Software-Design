namespace S4.HseCarShop.Models.PedalCar;

/// <summary>
/// Параметры педального автомобиля
/// </summary>
/// <param name="PedalSize">Размер педалей</param>
internal readonly record struct PedalCarParams(uint PedalSize);