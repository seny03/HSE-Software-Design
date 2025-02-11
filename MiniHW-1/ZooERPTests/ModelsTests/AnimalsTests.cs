using ZooERP.Models.Animals;

namespace ZooERP.Tests.Models
{
    public class AnimalsTests
    {
        [Fact]
        public void Monkey_Creation_ShouldInitializePropertiesCorrectly()
        {
            var monkey = new Monkey("Чарли", 5, true, 1, 7);

            Assert.Equal("Чарли", monkey.Name);
            Assert.Equal(5, monkey.Food);
            Assert.True(monkey.IsHealthy);
            Assert.Equal(1, monkey.Number);
            Assert.Equal((uint)7, monkey.Kindness);
        }

        [Fact]
        public void Rabbit_Creation_ShouldInitializePropertiesCorrectly()
        {
            var rabbit = new Rabbit("Флэш", 3, true, 2, 6);

            Assert.Equal("Флэш", rabbit.Name);
            Assert.Equal(3, rabbit.Food);
            Assert.True(rabbit.IsHealthy);
            Assert.Equal(2, rabbit.Number);
            Assert.Equal((uint)6, rabbit.Kindness);
        }

        [Fact]
        public void Tiger_Creation_ShouldInitializePropertiesCorrectly()
        {
            var tiger = new Tiger("Шерхан", 10, true, 3);

            Assert.Equal("Шерхан", tiger.Name);
            Assert.Equal(10, tiger.Food);
            Assert.True(tiger.IsHealthy);
            Assert.Equal(3, tiger.Number);
        }

        [Fact]
        public void Wolf_Creation_ShouldInitializePropertiesCorrectly()
        {
            var wolf = new Wolf("Акела", 8, true, 4);

            Assert.Equal("Акела", wolf.Name);
            Assert.Equal(8, wolf.Food);
            Assert.True(wolf.IsHealthy);
            Assert.Equal(4, wolf.Number);
        }

        [Fact]
        public void Monkey_InvalidKindness_ShouldThrowException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Monkey("Бобо", 5, true, 5, 11));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Monkey("Бобо", 5, true, 5, 999));
        }

        [Fact]
        public void Rabbit_InvalidKindness_ShouldThrowException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Rabbit("Снежок", 3, true, 6, 12));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Rabbit("Снежок", 3, true, 6, 999));
        }

        [Fact]
        public void Animals_ShouldImplementIAliveAndIInventory()
        {
            var monkey = new Monkey("Чарли", 5, true, 1, 7);
            var rabbit = new Rabbit("Флэш", 3, true, 2, 6);
            var tiger = new Tiger("Шерхан", 10, true, 3);
            var wolf = new Wolf("Акела", 8, true, 4);

            Assert.True(monkey is ZooERP.Models.Abstractions.IAlive);
            Assert.True(monkey is ZooERP.Models.Abstractions.IInventory);
            Assert.True(rabbit is ZooERP.Models.Abstractions.IAlive);
            Assert.True(rabbit is ZooERP.Models.Abstractions.IInventory);
            Assert.True(tiger is ZooERP.Models.Abstractions.IAlive);
            Assert.True(tiger is ZooERP.Models.Abstractions.IInventory);
            Assert.True(wolf is ZooERP.Models.Abstractions.IAlive);
            Assert.True(wolf is ZooERP.Models.Abstractions.IInventory);
        }
    }
}
