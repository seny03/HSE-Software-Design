using ZooERP.Models.Abstractions;
using ZooERP.Models.Inventory;

namespace ZooERP.Tests.Models
{
    public class InventoryTests
    {
        [Fact]
        public void Table_Creation_ShouldInitializePropertiesCorrectly()
        {
            var table = new Table(101);

            Assert.Equal(101, table.Number);
        }

        [Fact]
        public void Computer_Creation_ShouldInitializePropertiesCorrectly()
        {
            var computer = new Computer(202);

            Assert.Equal(202, computer.Number);
        }

        [Fact]
        public void Table_ShouldImplementIInventory()
        {
            var table = new Table(101);
            Assert.True(table is IInventory);
        }

        [Fact]
        public void Computer_ShouldImplementIInventory()
        {
            var computer = new Computer(202);
            Assert.True(computer is IInventory);
        }
    }
}
