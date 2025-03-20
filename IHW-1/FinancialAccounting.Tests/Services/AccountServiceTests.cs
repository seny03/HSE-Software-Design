using System;
using Xunit;
using Moq;
using FinancialAccounting.Domain;
using FinancialAccounting.Persistence;
using FinancialAccounting.Services;
using System.Collections.Generic;
using System.Linq;

namespace FinancialAccounting.Tests.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<IRepository<BankAccount>> _mockRepository;
        private readonly AccountService _service;

        public AccountServiceTests()
        {
            _mockRepository = new Mock<IRepository<BankAccount>>();
            _service = new AccountService(_mockRepository.Object);
        }

        [Fact]
        public void GetRepository_ReturnsRepository()
        {
            
            var repository = _service.GetRepository();

            
            Assert.Same(_mockRepository.Object, repository);
        }

        [Fact]
        public void Create_WithValidParameters_CreatesAndAddsAccount()
        {
            
            string name = "TestAccount";
            decimal balance = 1000m;
            BankAccount addedAccount = null;

            _mockRepository.Setup(r => r.Add(It.IsAny<BankAccount>()))
                .Callback<BankAccount>(account => addedAccount = account);

            
            var result = _service.Create(name, balance);

            
            Assert.NotNull(result);
            Assert.Equal(name, result.Name);
            Assert.Equal(balance, result.Balance);
            Assert.NotEqual(Guid.Empty, result.Id);
            
            _mockRepository.Verify(r => r.Add(It.IsAny<BankAccount>()), Times.Once);
            Assert.Same(result, addedAccount);
        }

        [Fact]
        public void Create_WithSpecifiedId_CreatesAccountWithThatId()
        {
            
            string name = "TestAccount";
            decimal balance = 1000m;
            Guid id = Guid.NewGuid();

            
            var result = _service.Create(name, balance, id);

            
            Assert.Equal(id, result.Id);
            _mockRepository.Verify(r => r.Add(It.Is<BankAccount>(a => a.Id == id)), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidName_ThrowsArgumentException(string invalidName)
        {
            
            decimal balance = 1000m;

            
            Assert.Throws<ArgumentException>(() => _service.Create(invalidName, balance));
            _mockRepository.Verify(r => r.Add(It.IsAny<BankAccount>()), Times.Never);
        }

        [Fact]
        public void Update_WithValidParameters_UpdatesAccount()
        {
            
            var id = Guid.NewGuid();
            var existingAccount = new BankAccount("OldName", 500m) { Id = id };
            string newName = "NewName";
            decimal newBalance = 1500m;

            _mockRepository.Setup(r => r.Get(id)).Returns(existingAccount);

            
            _service.Update(id, newName, newBalance);

            
            Assert.Equal(newName, existingAccount.Name);
            Assert.Equal(newBalance, existingAccount.Balance);
            _mockRepository.Verify(r => r.Update(existingAccount), Times.Once);
        }

        [Fact]
        public void Update_WithNonExistingId_ThrowsInvalidOperationException()
        {
            
            var id = Guid.NewGuid();
            _mockRepository.Setup(r => r.Get(id)).Returns((BankAccount)null);

            
            Assert.Throws<InvalidOperationException>(() => _service.Update(id, "NewName", 1000m));
            _mockRepository.Verify(r => r.Update(It.IsAny<BankAccount>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WithInvalidName_ThrowsArgumentException(string invalidName)
        {
            
            var id = Guid.NewGuid();
            decimal balance = 1000m;

            
            Assert.Throws<ArgumentException>(() => _service.Update(id, invalidName, balance));
            _mockRepository.Verify(r => r.Update(It.IsAny<BankAccount>()), Times.Never);
        }

        [Fact]
        public void Delete_ExistingAccount_DeletesAccount()
        {
            
            var id = Guid.NewGuid();
            var existingAccount = new BankAccount("TestAccount", 1000m) { Id = id };
            _mockRepository.Setup(r => r.Get(id)).Returns(existingAccount);

            
            _service.Delete(id);

            
            _mockRepository.Verify(r => r.Delete(id), Times.Once);
        }

        [Fact]
        public void Delete_NonExistingAccount_ThrowsInvalidOperationException()
        {
            
            var id = Guid.NewGuid();
            _mockRepository.Setup(r => r.Get(id)).Returns((BankAccount)null);

            
            Assert.Throws<InvalidOperationException>(() => _service.Delete(id));
            _mockRepository.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public void GetAll_ReturnsAllAccounts()
        {
            
            var accounts = new List<BankAccount>
            {
                new BankAccount("Account1", 1000m) { Id = Guid.NewGuid() },
                new BankAccount("Account2", 2000m) { Id = Guid.NewGuid() },
                new BankAccount("Account3", 3000m) { Id = Guid.NewGuid() }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(accounts);

            
            var result = _service.GetAll().ToList();

            
            Assert.Equal(accounts.Count, result.Count);
            foreach (var account in accounts)
            {
                Assert.Contains(result, a => a.Id == account.Id);
            }
        }
    }
}
