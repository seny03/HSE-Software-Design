using ZooWebApp.Domain.Entities;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Application.Services;

public interface IZooStatisticsService
{
    Task<int> GetTotalAnimalCountAsync();
    Task<int> GetAvailableEnclosuresCountAsync();
    Task<int> GetOccupiedEnclosuresCountAsync();
    Task<IDictionary<Species, int>> GetAnimalsBySpeciesAsync();
    Task<IDictionary<HealthStatus, int>> GetAnimalsByHealthStatusAsync();
    Task<IDictionary<string, int>> GetAnimalsByEnclosureAsync();
    Task<ZooSummary> GetZooSummaryAsync();
}

public record ZooSummary
{
    public int TotalAnimals { get; init; }
    public int HealthyAnimals { get; init; }
    public int SickAnimals { get; init; }
    public int TotalEnclosures { get; init; }
    public int AvailableEnclosures { get; init; }
    public int OccupiedEnclosures { get; init; }
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;
}
