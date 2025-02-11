namespace S4.HseCarShop.Models;

/// <summary>
/// Аргументы для проверки подходит ли машина пользователю по физическим параметрам
/// </summary>
/// <param name="HandStrength">Сила рук</param>
/// <param name="LegStrength">Сила ног</param>
/// <remarks>
/// Забавный факт, если не добавить readonly, то запись будет мутабельной :)
/// </remarks>
internal readonly record struct CarAvailabilityParams(uint HandStrength, uint LegStrength);