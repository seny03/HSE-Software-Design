using Microsoft.Extensions.DependencyInjection;
using S4.HseCarShop.Models;
using S4.HseCarShop.Models.HandCar;
using S4.HseCarShop.Models.HybridCar;
using S4.HseCarShop.Models.PedalCar;
using S4.HseCarShop.Services;
using S4.HseCarShop.Services.Abstractions;
using S4.HseCarShop.Services.HandCars;
using S4.HseCarShop.Services.HybridCars;
using S4.HseCarShop.Services.PedalCars;

namespace S4.HseCarShop;

internal class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();

        services.AddSingleton<CarStorage>();
        services.AddSingleton<ICarProvider>(sp => sp.GetRequiredService<CarStorage>());
        services.AddSingleton<CustomersStorage>();
        services.AddSingleton<ICustomerProvider>(sp => sp.GetRequiredService<CustomersStorage>());

        services.AddSingleton<ICarFactory<HandCar, HandCarParams>, HandCarFactory>();
        services.AddSingleton<IEngineFactory<HandEngine, HandEngineParams>, HandEngineFactory>();
        services.AddSingleton<ICarFactory<PedalCar, PedalCarParams>, PedalCarFactory>();
        services.AddSingleton<IEngineFactory<PedalEngine, PedalEngineParams>, PedalEngineFactory>();
        services.AddSingleton<ICarFactory<HybridCar, HybridCarParams>, HybridCarFactory>();
        services.AddSingleton<IEngineFactory<HybridEngine, HybridEngineParams>, HybridEngineFactory>();

        services.AddSingleton<ICarAvailabilityService, CarAvailabilityService>();
        services.AddSingleton<IAvailabilityCheck, HandCarAvailabilityCheck>();
        services.AddSingleton<IAvailabilityCheck, PedalCarAvailabilityCheck>();
        services.AddSingleton<IAvailabilityCheck, HybridCarAvailabilityCheck>();

        services.AddSingleton<CarShopService>();

        var serviceProvider = services.BuildServiceProvider();

        var customerStorage = serviceProvider.GetRequiredService<CustomersStorage>();

        var customers = new[]
{
            new Customer(name: "Ivan", legStrength: 10, handStrength: 1),
            new Customer(name : "Petr", legStrength : 1, handStrength : 10),
            new Customer(name : "Sidr", legStrength : 1, handStrength : 1),
            new Customer(name : "Sidr", legStrength : 10, handStrength : 10),
        };

        foreach (var customer in customers)
            customerStorage.AddCustomer(customer);

        var handCarFactory = serviceProvider.GetRequiredService<ICarFactory<HandCar, HandCarParams>>();
        var pedalCarFactory = serviceProvider.GetRequiredService<ICarFactory<PedalCar, PedalCarParams>>();
        var hybridCarFactory = serviceProvider.GetRequiredService<ICarFactory<HybridCar, HybridCarParams>>();

        var carStorage = serviceProvider.GetRequiredService<CarStorage>();

        carStorage.AddCar(pedalCarFactory, new PedalCarParams { PedalSize = 2 });
        carStorage.AddCar(pedalCarFactory, new PedalCarParams { PedalSize = 3 });
        carStorage.AddCar(handCarFactory, new HandCarParams { GripsType = GripsType.Rubber });
        carStorage.AddCar(handCarFactory, new HandCarParams { GripsType = GripsType.Silicone });

        var hseCarShop = serviceProvider.GetRequiredService<CarShopService>();

        Console.WriteLine("=== Покупатели ===");
        foreach (var customer in customers)
            Console.WriteLine(customer);

        Console.WriteLine("\n=== Продажа автомобилей ===\n");
        hseCarShop.SellCars();

        Console.WriteLine("=== Покупатели ===");
        foreach (var customer in customers)
            Console.WriteLine(customer);
    }
}
