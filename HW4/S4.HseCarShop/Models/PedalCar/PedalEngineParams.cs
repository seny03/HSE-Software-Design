namespace S4.HseCarShop.Models.PedalCar;

/// <summary>
/// Параметры педального двигателя
/// </summary>
/// <param name="PedalSize">Размер педалей</param>
internal readonly record struct PedalEngineParams(uint PedalSize);