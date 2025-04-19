using Microsoft.AspNetCore.Mvc;
using ZooWebApp.Application.Services;

namespace ZooWebApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ZooStatisticsController : ControllerBase
{
    private readonly IZooStatisticsService _statisticsService;

    public ZooStatisticsController(IZooStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _statisticsService.GetZooSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("animals/count")]
    public async Task<IActionResult> GetTotalAnimalCount()
    {
        var count = await _statisticsService.GetTotalAnimalCountAsync();
        return Ok(count);
    }

    [HttpGet("enclosures/available")]
    public async Task<IActionResult> GetAvailableEnclosures()
    {
        var count = await _statisticsService.GetAvailableEnclosuresCountAsync();
        return Ok(count);
    }

    [HttpGet("enclosures/occupied")]
    public async Task<IActionResult> GetOccupiedEnclosures()
    {
        var count = await _statisticsService.GetOccupiedEnclosuresCountAsync();
        return Ok(count);
    }

    [HttpGet("animals/by-species")]
    public async Task<IActionResult> GetAnimalsBySpecies()
    {
        var bySpecies = await _statisticsService.GetAnimalsBySpeciesAsync();
        return Ok(bySpecies);
    }

    [HttpGet("animals/by-health")]
    public async Task<IActionResult> GetAnimalsByHealthStatus()
    {
        var byHealth = await _statisticsService.GetAnimalsByHealthStatusAsync();
        return Ok(byHealth);
    }

    [HttpGet("animals/by-enclosure")]
    public async Task<IActionResult> GetAnimalsByEnclosure()
    {
        var byEnclosure = await _statisticsService.GetAnimalsByEnclosureAsync();
        return Ok(byEnclosure);
    }
}
