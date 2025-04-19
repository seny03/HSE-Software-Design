using ZooWebApp.Domain.Common;
using ZooWebApp.Domain.Common.Interfaces;
using ZooWebApp.Domain.Entities;

namespace ZooWebApp.Infrastructure.Repositories;

public class InMemoryAnimalRepository : IAnimalRepository
{
    private readonly List<Animal> _animals = new();
    private int _nextId = 1;

    public Task<Animal?> GetByIdAsync(int id)
    {
        return Task.FromResult(_animals.FirstOrDefault(a => a.Id == id));
    }

    public Task<IEnumerable<Animal>> GetAllAsync()
    {
        return Task.FromResult((IEnumerable<Animal>)_animals);
    }

    public Task AddAsync(Animal animal)
    {
        // Set the ID for the new animal
        typeof(EntityBase).GetProperty("Id")?.SetValue(animal, _nextId++);
        _animals.Add(animal);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Animal animal)
    {
        var index = _animals.FindIndex(a => a.Id == animal.Id);
        if (index != -1)
        {
            _animals[index] = animal;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var animal = _animals.FirstOrDefault(a => a.Id == id);
        if (animal != null)
        {
            _animals.Remove(animal);
        }
        return Task.CompletedTask;
    }
}
