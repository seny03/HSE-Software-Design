using ZooWebApp.Domain.Entities;
using ZooWebApp.Domain.Common.Interfaces;

namespace ZooWebApp.Application.Services;

public class AnimalTransferService
{
    private readonly IAnimalRepository _animalRepository;
    private readonly IEnclosureRepository _enclosureRepository;

    public AnimalTransferService(
        IAnimalRepository animalRepository,
        IEnclosureRepository enclosureRepository)
    {
        _animalRepository = animalRepository;
        _enclosureRepository = enclosureRepository;
    }

    public async Task TransferAnimalAsync(int animalId, int newEnclosureId)
    {
        var animal = await _animalRepository.GetByIdAsync(animalId);
        var newEnclosure = await _enclosureRepository.GetByIdAsync(newEnclosureId);

        if (animal == null || newEnclosure == null)
            throw new ArgumentException("Invalid animal or enclosure ID");

        if (newEnclosure.CurrentOccupancy >= newEnclosure.MaxCapacity)
            throw new InvalidOperationException("Enclosure is full");

        if (animal.Species != newEnclosure.SpeciesType)
            throw new InvalidOperationException("Enclosure type does not match animal species");

        animal.MoveToEnclosure(newEnclosure);
        await _animalRepository.UpdateAsync(animal);
        await _enclosureRepository.UpdateAsync(newEnclosure);
    }
}
