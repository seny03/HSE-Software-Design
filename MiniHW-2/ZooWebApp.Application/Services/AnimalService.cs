using ZooWebApp.Domain.Common.Interfaces;
using ZooWebApp.Domain.Entities;

namespace ZooWebApp.Application.Services;

public class AnimalService : IAnimalService
{
    private readonly IAnimalRepository _animalRepository;

    public AnimalService(IAnimalRepository animalRepository)
    {
        _animalRepository = animalRepository;
    }

    public async Task<Animal?> GetAnimalByIdAsync(int id)
    {
        return await _animalRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Animal>> GetAllAnimalsAsync()
    {
        return await _animalRepository.GetAllAsync();
    }

    public async Task AddAnimalAsync(Animal animal)
    {
        await _animalRepository.AddAsync(animal);
    }

    public async Task UpdateAnimalAsync(Animal animal)
    {
        var existingAnimal = await _animalRepository.GetByIdAsync(animal.Id);
        if (existingAnimal != null)
        {
            // Update the existing animal with values from the incoming animal
            existingAnimal.Update(
                animal.Species,
                animal.Name,
                animal.BirthDate,
                animal.Gender,
                animal.FavoriteFood,
                animal.HealthStatus
            );
            
            // If the incoming animal has an enclosure, move the existing animal to that enclosure
            if (animal.Enclosure != null && existingAnimal.Enclosure?.Id != animal.Enclosure.Id)
            {
                existingAnimal.MoveToEnclosure(animal.Enclosure);
            }
            
            await _animalRepository.UpdateAsync(existingAnimal);
        }
    }

    public async Task DeleteAnimalAsync(int id)
    {
        await _animalRepository.DeleteAsync(id);
    }
}
