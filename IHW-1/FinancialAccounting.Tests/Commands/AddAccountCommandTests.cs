using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FinancialAccounting.Domain;
using FinancialAccounting.Services;
using FinancialAccounting.Persistence;

namespace FinancialAccounting.Tests.Commands
{
    public class AddAccountCommandTests
    {
        [Fact]
        public async Task ExecuteAsync_CallsAccountServiceCreate()
        {
            
            var mockRepo = new Mock<IRepository<BankAccount>>();
            BankAccount addedAccount = null;
            
            mockRepo.Setup(r => r.Add(It.IsAny<BankAccount>()))
                .Callback<BankAccount>(a => addedAccount = a);
            
            var accountService = new AccountService(mockRepo.Object);
            string accountName = "TestAccount";
            decimal balance = 1000m;
            
            var command = new AddAccountCommand(accountService, accountName, balance);
            
            
            await command.ExecuteAsync();
            
            
            mockRepo.Verify(r => r.Add(It.IsAny<BankAccount>()), Times.Once);
            Assert.NotNull(addedAccount);
            Assert.Equal(accountName, addedAccount.Name);
            Assert.Equal(balance, addedAccount.Balance);
        }
        
        [Fact]
        public async Task ExecuteAsync_WithInvalidName_ThrowsArgumentException()
        {
            
            var mockRepo = new Mock<IRepository<BankAccount>>();
            var accountService = new AccountService(mockRepo.Object);
            string invalidName = "";
            decimal balance = 1000m;
            
            var command = new AddAccountCommand(accountService, invalidName, balance);
            
            
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => command.ExecuteAsync());
            Assert.Contains("Account name cannot be empty", exception.Message);
        }
        
        [Fact]
        public async Task ExecuteAsync_WithNegativeBalance_ThrowsArgumentException()
        {
            
            var mockRepo = new Mock<IRepository<BankAccount>>();
            var accountService = new AccountService(mockRepo.Object);
            string accountName = "TestAccount";
            decimal negativeBalance = -100m;
            
            var command = new AddAccountCommand(accountService, accountName, negativeBalance);
            
            
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => command.ExecuteAsync());
            Assert.Contains("Value does not fall within the expected", exception.Message);
        }
    }
}
