using System.Diagnostics;
using ZooERP.Models.Abstractions;

namespace ZooERP.Services;

public class Zoo
{
    private List<Animal> Animals { get; } = new List<Animal>();
    private List<Thing> Inventory { get; } = new List<Thing>();
    private readonly VeterinaryClinic _clinic;

    public Zoo(VeterinaryClinic clinic)
    {
        _clinic = clinic;
    }

    public bool AddAnimal(Animal animal)
    {
        if (_clinic.CheckHealth(animal))
        {
            Animals.Add(animal);
            Debug.WriteLine($"{animal.Name} принят в зоопарк.");
            return true;
        }

        Debug.WriteLine($"{animal.Name} не прошел ветеринарный осмотр.");
        return false;
    }

    public bool AddInventory(Thing item)
    {
        Inventory.Add(item);
        Debug.WriteLine($"Добавлена вещь с номером {item.Number}.");
        return true;
    }

    public int GetTotalFoodConsumption()
    {
        return Animals.Sum(a => a.Food);
    }

    public IEnumerable<Animal> GetContactZooAnimals()
    {
        return Animals.OfType<Herbo>().Where(a => a.Kindness > 5);
    }

    public IEnumerable<Thing> GetInventoryList()
    {
        return Inventory;
    }

    public override string ToString()
    {
        var animalInfo = Animals.Count > 0
            ? string.Join("\n", Animals.Select(a => $"{a.Name} ({a.GetType().Name}), еда: {a.Food} кг"))
            : "Нет животных.";

        var inventoryInfo = Inventory.Count > 0
            ? string.Join("\n", Inventory.Select(i => $"Инвентарь №{i.Number} ({i.GetType().Name})"))
            : "Нет инвентаря.";

        return $"🌿 Зоопарк:\n\n🐾 Животные:\n{animalInfo}\n\n📦 Инвентарь:\n{inventoryInfo}\n\n" +
            $"🍽 Общее потребление еды: {GetTotalFoodConsumption()} кг в день.";
    }
}
