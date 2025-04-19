using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Application.Services;

public interface IFeedingScheduleService
{
    Task<IEnumerable<FeedingSchedule>> GetAllSchedulesAsync();
    Task<IEnumerable<FeedingSchedule>> GetSchedulesByAnimalIdAsync(int animalId);
    Task AddFeedingScheduleAsync(int animalId, FeedingSchedule schedule);
    Task MarkFeedingCompleteAsync(int animalId, TimeSpan time);
}
