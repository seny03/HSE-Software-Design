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
    public class OperationServiceTests
    {
        private readonly Mock<IRepository<Operation>> _mockOperationRepo;
        private readonly Mock<IRepository<BankAccount>> _mockAccountRepo;
        private readonly OperationService _service;
        private readonly Guid _accountId;
        private readonly Guid _categoryId;
        private readonly BankAccount _account;

        public OperationServiceTests()
        {
            _mockOperationRepo = new Mock<IRepository<Operation>>();
            _mockAccountRepo = new Mock<IRepository<BankAccount>>();
            _service = new OperationService(_mockOperationRepo.Object, _mockAccountRepo.Object);
            
            _accountId = Guid.NewGuid();
            _categoryId = Guid.NewGuid();
            _account = new BankAccount("TestAccount", 1000m) { Id = _accountId };
            
            _mockAccountRepo.Setup(r => r.Get(_accountId)).Returns(_account);
        }

        [Fact]
        public void GetRepository_ReturnsRepository()
        {
            
            var repository = _service.GetRepository();

            
            Assert.Same(_mockOperationRepo.Object, repository);
        }

        [Fact]
        public void Create_WithValidParameters_CreatesAndAddsOperation()
        {
            
            OperationType type = OperationType.Income;
            decimal amount = 500m;
            DateTime date = DateTime.Now;
            string description = "Test operation";
            Operation addedOperation = null;

            _mockOperationRepo.Setup(r => r.Add(It.IsAny<Operation>()))
                .Callback<Operation>(op => addedOperation = op);

            
            var result = _service.Create(type, _accountId, amount, date, _categoryId, description);

            
            Assert.NotNull(result);
            Assert.Equal(type, result.Type);
            Assert.Equal(_accountId, result.BankAccountId);
            Assert.Equal(amount, result.Amount);
            Assert.Equal(date, result.Date);
            Assert.Equal(_categoryId, result.CategoryId);
            Assert.Equal(description, result.Description);
            Assert.NotEqual(Guid.Empty, result.Id);
            
            _mockOperationRepo.Verify(r => r.Add(It.IsAny<Operation>()), Times.Once);
            Assert.Same(result, addedOperation);
            
            
            Assert.Equal(1500m, _account.Balance);
        }

        [Fact]
        public void Create_WithExpenseType_DecreasesAccountBalance()
        {
            
            OperationType type = OperationType.Expense;
            decimal amount = 300m;
            DateTime date = DateTime.Now;

            
            _service.Create(type, _accountId, amount, date, _categoryId);

            
            Assert.Equal(700m, _account.Balance);
        }

        [Fact]
        public void Create_WithSkipBalanceUpdate_DoesNotChangeAccountBalance()
        {
            
            OperationType type = OperationType.Income;
            decimal amount = 500m;
            DateTime date = DateTime.Now;
            decimal initialBalance = _account.Balance;

            
            _service.Create(type, _accountId, amount, date, _categoryId, skipBalanceUpdate: true);

            
            Assert.Equal(initialBalance, _account.Balance);
        }

        [Fact]
        public void Create_WithSpecifiedId_CreatesOperationWithThatId()
        {
            
            OperationType type = OperationType.Income;
            decimal amount = 500m;
            DateTime date = DateTime.Now;
            Guid id = Guid.NewGuid();

            
            var result = _service.Create(type, _accountId, amount, date, _categoryId, id: id);

            
            Assert.Equal(id, result.Id);
            _mockOperationRepo.Verify(r => r.Add(It.Is<Operation>(o => o.Id == id)), Times.Once);
        }

        [Fact]
        public void Create_WithNonExistingAccount_ThrowsInvalidOperationException()
        {
            
            var nonExistingAccountId = Guid.NewGuid();
            _mockAccountRepo.Setup(r => r.Get(nonExistingAccountId)).Returns((BankAccount)null);

            
            Assert.Throws<InvalidOperationException>(() => 
                _service.Create(OperationType.Income, nonExistingAccountId, 100m, DateTime.Now, _categoryId));
        }

        [Fact]
        public void Update_WithSameAccount_UpdatesOperationAndAccountBalance()
        {
            
            var operationId = Guid.NewGuid();
            var originalOperation = new Operation(
                operationId, 
                OperationType.Income, 
                _accountId, 
                200m, 
                DateTime.Now, 
                _categoryId, 
                "Original description");
            
            _mockOperationRepo.Setup(r => r.Get(operationId)).Returns(originalOperation);
            
            OperationType newType = OperationType.Expense;
            decimal newAmount = 300m;
            DateTime newDate = DateTime.Now.AddDays(1);
            string newDescription = "Updated description";

            
            _service.Update(operationId, newType, _accountId, newAmount, newDate, _categoryId, newDescription);

            
            
            
            Assert.Equal(500m, _account.Balance);
            
            Assert.Equal(newType, originalOperation.Type);
            Assert.Equal(_accountId, originalOperation.BankAccountId);
            Assert.Equal(newAmount, originalOperation.Amount);
            Assert.Equal(newDate, originalOperation.Date);
            Assert.Equal(_categoryId, originalOperation.CategoryId);
            Assert.Equal(newDescription, originalOperation.Description);
            
            _mockOperationRepo.Verify(r => r.Update(originalOperation), Times.Once);
        }

        [Fact]
        public void Update_WithDifferentAccount_UpdatesBothAccountBalances()
        {
            
            var operationId = Guid.NewGuid();
            var oldAccountId = Guid.NewGuid();
            var oldAccount = new BankAccount("OldAccount", 1000m) { Id = oldAccountId };
            
            var originalOperation = new Operation(
                operationId, 
                OperationType.Income, 
                oldAccountId, 
                200m, 
                DateTime.Now, 
                _categoryId);
            
            _mockOperationRepo.Setup(r => r.Get(operationId)).Returns(originalOperation);
            _mockAccountRepo.Setup(r => r.Get(oldAccountId)).Returns(oldAccount);
            
            
            _service.Update(operationId, OperationType.Expense, _accountId, 300m, DateTime.Now, _categoryId);

            
            
            Assert.Equal(800m, oldAccount.Balance);
            
            
            Assert.Equal(700m, _account.Balance);
        }

        [Fact]
        public void Update_WithNonExistingOperation_ThrowsInvalidOperationException()
        {
            
            var nonExistingOperationId = Guid.NewGuid();
            _mockOperationRepo.Setup(r => r.Get(nonExistingOperationId)).Returns((Operation)null);

            
            Assert.Throws<InvalidOperationException>(() => 
                _service.Update(nonExistingOperationId, OperationType.Income, _accountId, 100m, DateTime.Now, _categoryId));
        }

        [Fact]
        public void Update_WithNonExistingAccount_ThrowsInvalidOperationException()
        {
            
            var operationId = Guid.NewGuid();
            var originalOperation = new Operation(
                operationId, 
                OperationType.Income, 
                _accountId, 
                200m, 
                DateTime.Now, 
                _categoryId);
            
            _mockOperationRepo.Setup(r => r.Get(operationId)).Returns(originalOperation);
            
            var nonExistingAccountId = Guid.NewGuid();
            _mockAccountRepo.Setup(r => r.Get(nonExistingAccountId)).Returns((BankAccount)null);

            
            Assert.Throws<InvalidOperationException>(() => 
                _service.Update(operationId, OperationType.Income, nonExistingAccountId, 100m, DateTime.Now, _categoryId));
        }

        [Fact]
        public void Delete_ExistingOperation_DeletesOperationAndUpdatesAccountBalance()
        {
            
            var operationId = Guid.NewGuid();
            var operation = new Operation(
                operationId, 
                OperationType.Income, 
                _accountId, 
                200m, 
                DateTime.Now, 
                _categoryId);
            
            _mockOperationRepo.Setup(r => r.Get(operationId)).Returns(operation);

            
            _service.Delete(operationId);

            
            
            Assert.Equal(800m, _account.Balance);
            _mockOperationRepo.Verify(r => r.Delete(operationId), Times.Once);
        }

        [Fact]
        public void Delete_NonExistingOperation_ThrowsInvalidOperationException()
        {
            
            var nonExistingOperationId = Guid.NewGuid();
            _mockOperationRepo.Setup(r => r.Get(nonExistingOperationId)).Returns((Operation)null);

            
            Assert.Throws<InvalidOperationException>(() => _service.Delete(nonExistingOperationId));
            _mockOperationRepo.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public void BalanceDifference_CalculatesCorrectly()
        {
            
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 1, 31);
            
            var operations = new List<Operation>
            {
                new Operation(OperationType.Income, _accountId, 500m, new DateTime(2023, 1, 5), _categoryId),
                new Operation(OperationType.Expense, _accountId, 200m, new DateTime(2023, 1, 10), _categoryId),
                new Operation(OperationType.Income, _accountId, 300m, new DateTime(2023, 1, 15), _categoryId),
                new Operation(OperationType.Expense, _accountId, 100m, new DateTime(2023, 1, 20), _categoryId),
                
                new Operation(OperationType.Income, _accountId, 1000m, new DateTime(2023, 2, 1), _categoryId),
                
                new Operation(OperationType.Income, Guid.NewGuid(), 2000m, new DateTime(2023, 1, 15), _categoryId)
            };
            
            _mockOperationRepo.Setup(r => r.GetAll()).Returns(operations);

            
            var result = _service.BalanceDifference(_accountId, startDate, endDate);

            
            
            Assert.Equal(500m, result);
        }

        [Fact]
        public void GroupByCategory_ReturnsCorrectGrouping()
        {
            
            var category1Id = Guid.NewGuid();
            var category2Id = Guid.NewGuid();
            var category3Id = Guid.NewGuid();
            
            var category1 = new Category(category1Id, "Category1", CategoryType.Income);
            var category2 = new Category(category2Id, "Category2", CategoryType.Expense);
            var category3 = new Category(category3Id, "Category3", CategoryType.Income);
            
            var operations = new List<Operation>
            {
                new Operation(OperationType.Income, _accountId, 500m, DateTime.Now, category1Id),
                new Operation(OperationType.Income, _accountId, 300m, DateTime.Now, category1Id),
                new Operation(OperationType.Expense, _accountId, 200m, DateTime.Now, category2Id),
                new Operation(OperationType.Expense, _accountId, 100m, DateTime.Now, category2Id),
                new Operation(OperationType.Income, _accountId, 400m, DateTime.Now, category3Id),
                
                new Operation(OperationType.Income, Guid.NewGuid(), 1000m, DateTime.Now, category1Id)
            };
            
            var categories = new List<Category> { category1, category2, category3 };
            
            _mockOperationRepo.Setup(r => r.GetAll()).Returns(operations);
            
            
            var categoryRepo = new FinancialAccounting.Persistence.InMemoryRepository<Category>(c => c.Id);
            foreach (var category in categories)
            {
                categoryRepo.Add(category);
            }
            var categoryService = new CategoryService(categoryRepo);

            
            var result = _service.GroupByCategory(_accountId, categoryService);

            
            Assert.Equal(3, result.Count);
            Assert.Equal(800m, result["Category1"]); 
            Assert.Equal(-300m, result["Category2"]); 
            Assert.Equal(400m, result["Category3"]); 
        }

        [Fact]
        public void GetAll_ReturnsAllOperations()
        {
            
            var operations = new List<Operation>
            {
                new Operation(OperationType.Income, _accountId, 100m, DateTime.Now, _categoryId) { Id = Guid.NewGuid() },
                new Operation(OperationType.Expense, _accountId, 50m, DateTime.Now, _categoryId) { Id = Guid.NewGuid() },
                new Operation(OperationType.Income, _accountId, 200m, DateTime.Now, _categoryId) { Id = Guid.NewGuid() }
            };

            _mockOperationRepo.Setup(r => r.GetAll()).Returns(operations);

            
            var result = _service.GetAll().ToList();

            
            Assert.Equal(operations.Count, result.Count);
            foreach (var operation in operations)
            {
                Assert.Contains(result, o => o.Id == operation.Id);
            }
        }
    }
}
