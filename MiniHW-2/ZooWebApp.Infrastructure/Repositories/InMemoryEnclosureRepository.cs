using ZooWebApp.Domain.Common;
using ZooWebApp.Domain.Common.Interfaces;
using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Infrastructure.Repositories;

public class InMemoryEnclosureRepository : IEnclosureRepository
{
    private readonly List<Enclosure> _enclosures = new();
    private int _nextId = 1;

    public Task<Enclosure?> GetByIdAsync(int id)
    {
        return Task.FromResult(_enclosures.FirstOrDefault(e => e.Id == id));
    }

    public Task<IEnumerable<Enclosure>> GetAllAsync()
    {
        return Task.FromResult((IEnumerable<Enclosure>)_enclosures);
    }

    public Task AddAsync(Enclosure enclosure)
    {
        // Create a new enclosure with the ID assigned
        var newEnclosure = enclosure with { Id = _nextId++ };
        _enclosures.Add(newEnclosure);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Enclosure enclosure)
    {
        var index = _enclosures.FindIndex(e => e.Id == enclosure.Id);
        if (index != -1)
        {
            _enclosures[index] = enclosure;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var enclosure = _enclosures.FirstOrDefault(e => e.Id == id);
        if (enclosure != null)
        {
            _enclosures.Remove(enclosure);
        }
        return Task.CompletedTask;
    }
}
