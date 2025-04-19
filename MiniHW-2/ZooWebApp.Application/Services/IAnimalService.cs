using ZooWebApp.Domain.Entities;

namespace ZooWebApp.Application.Services;

public interface IAnimalService
{
    Task<Animal?> GetAnimalByIdAsync(int id);
    Task<IEnumerable<Animal>> GetAllAnimalsAsync();
    Task AddAnimalAsync(Animal animal);
    Task UpdateAnimalAsync(Animal animal);
    Task DeleteAnimalAsync(int id);
}
