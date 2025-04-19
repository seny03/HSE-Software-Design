using Microsoft.AspNetCore.Mvc;
using ZooWebApp.Application.Services;
using ZooWebApp.Domain.Entities;
using ZooWebApp.Presentation.Models;

namespace ZooWebApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IAnimalService _animalService;

    public AnimalsController(IAnimalService animalService)
    {
        _animalService = animalService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var animals = await _animalService.GetAllAnimalsAsync();
        var animalDtos = animals.Select(MapToDto);
        return Ok(animalDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var animal = await _animalService.GetAnimalByIdAsync(id);
        if (animal == null)
            return NotFound();
            
        return Ok(MapToDto(animal));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAnimalDto createDto)
    {
        var animal = new Animal(
            createDto.Species,
            createDto.Name,
            createDto.BirthDate,
            createDto.Gender,
            createDto.FavoriteFood
        );
        
        await _animalService.AddAnimalAsync(animal);
        return CreatedAtAction(nameof(GetById), new { id = animal.Id }, MapToDto(animal));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAnimalDto updateDto)
    {
        var existingAnimal = await _animalService.GetAnimalByIdAsync(id);
        if (existingAnimal == null)
            return NotFound();
        
        // Update the existing animal directly
        existingAnimal.Update(
            updateDto.Species,
            updateDto.Name,
            updateDto.BirthDate,
            updateDto.Gender, 
            updateDto.FavoriteFood,
            updateDto.HealthStatus
        );
        
        await _animalService.UpdateAnimalAsync(existingAnimal);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var animal = await _animalService.GetAnimalByIdAsync(id);
        if (animal == null)
            return NotFound();
            
        await _animalService.DeleteAnimalAsync(id);
        return NoContent();
    }
    
    private static AnimalDto MapToDto(Animal animal)
    {
        return new AnimalDto
        {
            Id = animal.Id,
            Species = animal.Species,
            Name = animal.Name,
            BirthDate = animal.BirthDate,
            Gender = animal.Gender,
            FavoriteFood = animal.FavoriteFood,
            HealthStatus = animal.HealthStatus,
            Enclosure = animal.Enclosure != null
                ? new EnclosureDto
                {
                    Id = animal.Enclosure.Id,
                    Name = animal.Enclosure.Name,
                    MaxCapacity = animal.Enclosure.MaxCapacity,
                    CurrentOccupancy = animal.Enclosure.CurrentOccupancy
                }
                : null
        };
    }
}
