using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Domain.Common.Interfaces;

public interface IEnclosureRepository
{
    Task<Enclosure?> GetByIdAsync(int id);
    Task<IEnumerable<Enclosure>> GetAllAsync();
    Task AddAsync(Enclosure enclosure);
    Task UpdateAsync(Enclosure enclosure);
    Task DeleteAsync(int id);
}
