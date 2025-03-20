using System;
using System.Threading.Tasks;
using Xunit;
using Moq;

namespace FinancialAccounting.Tests.Commands
{
    public class TimingDecoratorTests
    {
        [Fact]
        public async Task ExecuteAsync_CallsInnerCommandExecuteAsync()
        {
            
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(c => c.ExecuteAsync()).Returns(Task.CompletedTask);
            
            var decorator = new TimingDecorator(mockCommand.Object);
            
            
            await decorator.ExecuteAsync();
            
            
            mockCommand.Verify(c => c.ExecuteAsync(), Times.Once);
        }
        
        [Fact]
        public async Task ExecuteAsync_CompletesEvenIfInnerCommandThrows()
        {
            
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(c => c.ExecuteAsync()).ThrowsAsync(new Exception("Test exception"));
            
            var decorator = new TimingDecorator(mockCommand.Object);
            
            
            await Assert.ThrowsAsync<Exception>(() => decorator.ExecuteAsync());
        }
    }
}
