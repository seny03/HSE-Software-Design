using HW2;

namespace S2.HseCarShop;

internal class Program
{
    static void Main(string[] args)
    {
        var customerStorage = new CustomersStorage();

        var customers = new[]
{
            new Customer(name: "Ivan", legsPower: 6, armsPower: 4),
            new Customer(name: "Petr", legsPower : 4, armsPower : 6),
            new Customer(name : "Sidr", legsPower : 6, armsPower : 6),
            new Customer(name : "Sidr", legsPower : 4, armsPower : 4),
        };

        foreach (var customer in customers)
            customerStorage.AddCustomer(customer);

        var pedalCarFactory = new PedalCarFactory();
        var handCarFactory = new HandCarFactory();

        var carService = new CarService();

        carService.AddCar(pedalCarFactory, new PedalEngineParams(pedalSize: 2));
        carService.AddCar(pedalCarFactory, new PedalEngineParams(pedalSize: 3));
        carService.AddCar(handCarFactory, EmptyEngineParams.Empty);
        carService.AddCar(handCarFactory, EmptyEngineParams.Empty);

        var hseCarShop = new HseCarService(carService, customerStorage);

        Console.WriteLine("[+] Покупатели");
        foreach (var customer in customers)
            Console.WriteLine(customer);

        Console.WriteLine("\n[+] Продажа автомобилей\n");
        hseCarShop.SellCars();

        Console.WriteLine("[+] Покупатели после покупки");
        foreach (var customer in customers)
            Console.WriteLine(customer);
    }
}