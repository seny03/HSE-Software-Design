using ZooERP.Models.Animals;
using ZooERP.Models.Inventory;
using ZooERP.Services;

namespace ZooERP.Tests.Services
{
    public class ZooTests
    {
        private readonly VeterinaryClinic _clinic;
        private readonly Zoo _zoo;

        public ZooTests()
        {
            _clinic = new VeterinaryClinic();
            _zoo = new Zoo(_clinic);
        }

        [Fact]
        public void AddAnimal_HealthyAnimal_ShouldBeAccepted()
        {
            var monkey = new Monkey("Чарли", 5, true, 1, 7);
            bool result = _zoo.AddAnimal(monkey);

            Assert.True(result);
            Assert.Contains(monkey, _zoo.GetAllAnimals());
        }

        [Fact]
        public void AddAnimal_UnhealthyAnimal_ShouldBeRejected()
        {
            var tiger = new Tiger("Шерхан", 10, false, 2);
            bool result = _zoo.AddAnimal(tiger);

            Assert.False(result);
            Assert.DoesNotContain(tiger, _zoo.GetAllAnimals());
        }

        [Fact]
        public void AddInventory_ShouldBeAddedToInventoryList()
        {
            var table = new Table(101);
            bool result = _zoo.AddInventory(table);

            Assert.True(result);
            Assert.Contains(table, _zoo.GetInventoryList());
        }

        [Fact]
        public void GetTotalFoodConsumption_ShouldReturnCorrectValue()
        {
            _zoo.AddAnimal(new Monkey("Чарли", 5, true, 1, 7));
            _zoo.AddAnimal(new Tiger("Шерхан", 10, true, 2));

            int totalFood = _zoo.GetTotalFoodConsumption();

            Assert.Equal(15, totalFood);
        }

        [Fact]
        public void GetContactZooAnimals_ShouldReturnOnlyFriendlyHerbivores()
        {
            var friendlyRabbit = new Rabbit("Флэш", 3, true, 3, 7);
            var unfriendlyRabbit = new Rabbit("Снежок", 2, true, 4, 3);
            var tiger = new Tiger("Шерхан", 10, true, 5);

            _zoo.AddAnimal(friendlyRabbit);
            _zoo.AddAnimal(unfriendlyRabbit);
            _zoo.AddAnimal(tiger);

            var contactZooAnimals = _zoo.GetContactZooAnimals().ToList();

            Assert.Single(contactZooAnimals);
            Assert.Contains(friendlyRabbit, contactZooAnimals);
        }
    }
}
