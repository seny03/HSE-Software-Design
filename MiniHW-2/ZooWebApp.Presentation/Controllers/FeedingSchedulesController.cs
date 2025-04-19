using Microsoft.AspNetCore.Mvc;
using ZooWebApp.Application.Services;
using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedingSchedulesController : ControllerBase
{
    private readonly IFeedingScheduleService _feedingScheduleService;

    public FeedingSchedulesController(IFeedingScheduleService feedingScheduleService)
    {
        _feedingScheduleService = feedingScheduleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var schedules = await _feedingScheduleService.GetAllSchedulesAsync();
        return Ok(schedules);
    }

    [HttpGet("animals/{animalId}")]
    public async Task<IActionResult> GetByAnimalId(int animalId)
    {
        try
        {
            var schedules = await _feedingScheduleService.GetSchedulesByAnimalIdAsync(animalId);
            return Ok(schedules);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("animals/{animalId}")]
    public async Task<IActionResult> AddSchedule(int animalId, [FromBody] FeedingSchedule schedule)
    {
        try
        {
            await _feedingScheduleService.AddFeedingScheduleAsync(animalId, schedule);
            return CreatedAtAction(nameof(GetByAnimalId), new { animalId }, schedule);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("animals/{animalId}/complete")]
    public async Task<IActionResult> MarkComplete(int animalId, [FromBody] TimeSpan time)
    {
        try
        {
            await _feedingScheduleService.MarkFeedingCompleteAsync(animalId, time);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
