using ZooWebApp.Domain.Entities;

namespace ZooWebApp.Domain.Common.Interfaces;

public interface IAnimalRepository
{
    Task<Animal?> GetByIdAsync(int id);
    Task<IEnumerable<Animal>> GetAllAsync();
    Task AddAsync(Animal animal);
    Task UpdateAsync(Animal animal);
    Task DeleteAsync(int id);
}
