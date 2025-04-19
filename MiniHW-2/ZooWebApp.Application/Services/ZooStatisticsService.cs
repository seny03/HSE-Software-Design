using ZooWebApp.Domain.Common.Interfaces;
using ZooWebApp.Domain.Entities;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Application.Services;

public class ZooStatisticsService : IZooStatisticsService
{
    private readonly IAnimalRepository _animalRepository;
    private readonly IEnclosureRepository _enclosureRepository;

    public ZooStatisticsService(
        IAnimalRepository animalRepository, 
        IEnclosureRepository enclosureRepository)
    {
        _animalRepository = animalRepository;
        _enclosureRepository = enclosureRepository;
    }

    public async Task<int> GetTotalAnimalCountAsync()
    {
        var animals = await _animalRepository.GetAllAsync();
        return animals.Count();
    }

    public async Task<int> GetAvailableEnclosuresCountAsync()
    {
        var enclosures = await _enclosureRepository.GetAllAsync();
        return enclosures.Count(e => e.CurrentOccupancy < e.MaxCapacity);
    }

    public async Task<int> GetOccupiedEnclosuresCountAsync()
    {
        var enclosures = await _enclosureRepository.GetAllAsync();
        return enclosures.Count(e => e.CurrentOccupancy > 0);
    }

    public async Task<IDictionary<Species, int>> GetAnimalsBySpeciesAsync()
    {
        var animals = await _animalRepository.GetAllAsync();
        return animals
            .GroupBy(a => a.Species)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<IDictionary<HealthStatus, int>> GetAnimalsByHealthStatusAsync()
    {
        var animals = await _animalRepository.GetAllAsync();
        return animals
            .GroupBy(a => a.HealthStatus)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<IDictionary<string, int>> GetAnimalsByEnclosureAsync()
    {
        var animals = await _animalRepository.GetAllAsync();
        return animals
            .GroupBy(a => a.Enclosure?.Name ?? "No Enclosure")
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<ZooSummary> GetZooSummaryAsync()
    {
        var animals = await _animalRepository.GetAllAsync();
        var enclosures = await _enclosureRepository.GetAllAsync();
        
        return new ZooSummary
        {
            TotalAnimals = animals.Count(),
            HealthyAnimals = animals.Count(a => a.HealthStatus == HealthStatus.Healthy),
            SickAnimals = animals.Count(a => a.HealthStatus == HealthStatus.Sick),
            TotalEnclosures = enclosures.Count(),
            AvailableEnclosures = enclosures.Count(e => e.CurrentOccupancy < e.MaxCapacity),
            OccupiedEnclosures = enclosures.Count(e => e.CurrentOccupancy > 0)
        };
    }
}
