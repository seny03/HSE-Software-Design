using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using ZooERP.Configuration;
using ZooERP.Models.Animals;
using ZooERP.Models.Inventory;
using ZooERP.Services;

class Program
{
    static void Main()
    {
        // Настройка DI-контейнера
        var serviceProvider = new ServiceCollection()
            .AddApplicationServices()
            .BuildServiceProvider();

        // Для отображения спецсимволов в консоли
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Перенаправление Debug.WriteLine в консоль
        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        Debug.AutoFlush = true;


        var zoo = serviceProvider.GetRequiredService<Zoo>();

        // Добавление животных
        zoo.AddAnimal(new Monkey("Чарли", 5, true, 0, 7));
        zoo.AddAnimal(new Monkey("Бобо", 8, true, 1, 6));
        zoo.AddAnimal(new Rabbit("Флэш", 3, true, 2, 4));
        zoo.AddAnimal(new Rabbit("Снежок", 2, false, 3, 3));
        zoo.AddAnimal(new Tiger("Шерхан", 10, true, 4));
        zoo.AddAnimal(new Tiger("Ричард", 6, true, 5));
        zoo.AddAnimal(new Wolf("Акела", 8, false, 6));
        zoo.AddAnimal(new Wolf("Грей", 5, true, 7));

        // Добавление инвентаря
        zoo.AddInventory(new Table(101));
        zoo.AddInventory(new Table(102));
        zoo.AddInventory(new Computer(201));
        zoo.AddInventory(new Computer(202));
        zoo.AddInventory(new Computer(203));

        // Вывод информации о зоопарке
        //Console.WriteLine(zoo);

        // Общее потребление еды
        Console.WriteLine("\n🍽 Общее потребление еды: " + zoo.GetTotalFoodConsumption() + " кг в день.");

        // Животные, доступные для контактного зоопарка
        Console.WriteLine("\n🐾 Животные для контактного зоопарка:");
        foreach (var animal in zoo.GetContactZooAnimals())
        {
            Console.WriteLine($"- {animal.Name} ({animal.GetType().Name})");
        }

        // Список инвентаря
        Console.WriteLine("\n📦 Список инвентаря:");
        foreach (var item in zoo.GetInventoryList())
        {
            Console.WriteLine($"- Инвентарь № {item.Number} ({item.GetType().Name})");
        }

        // Проверка работы ветеринарной клиники
        Console.WriteLine("\n🩺 Проверка здоровья животных:");
        foreach (var animal in zoo.GetAllAnimals())
        {
            Console.WriteLine($"- {animal.Name} ({animal.GetType().Name}): " + (animal.IsHealthy ? "✅ Здоров" : "❌ Требуется лечение"));
        }
    }
}
