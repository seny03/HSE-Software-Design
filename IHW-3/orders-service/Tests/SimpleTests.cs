using Xunit;

namespace OrdersService.Tests
{
    public class SimpleTests
    {
        [Fact]
        public void SimpleTest_True()
        {
            
            Assert.True(true);
        }

        [Fact]
        public void SimpleTest_Equal()
        {
            
            Assert.Equal(4, 2 + 2);
        }

        [Fact]
        public void SimpleTest_NotEqual()
        {
            
            Assert.NotEqual(5, 2 + 2);
        }
    }
} 