using System;
using Xunit;
using FinancialAccounting.Domain;

namespace FinancialAccounting.Tests.Domain
{
    public class OperationTests
    {
        [Fact]
        public void Constructor_WithAllParameters_CreatesOperation()
        {
            
            OperationType type = OperationType.Income;
            Guid accountId = Guid.NewGuid();
            decimal amount = 100m;
            DateTime date = DateTime.Now;
            Guid categoryId = Guid.NewGuid();
            string description = "Test description";

            
            var operation = new Operation(type, accountId, amount, date, categoryId, description);

            
            Assert.Equal(type, operation.Type);
            Assert.Equal(accountId, operation.BankAccountId);
            Assert.Equal(amount, operation.Amount);
            Assert.Equal(date, operation.Date);
            Assert.Equal(categoryId, operation.CategoryId);
            Assert.Equal(description, operation.Description);
            Assert.NotEqual(Guid.Empty, operation.Id);
        }

        [Fact]
        public void Constructor_WithoutDescription_CreatesOperationWithEmptyDescription()
        {
            
            OperationType type = OperationType.Expense;
            Guid accountId = Guid.NewGuid();
            decimal amount = 50m;
            DateTime date = DateTime.Now;
            Guid categoryId = Guid.NewGuid();

            
            var operation = new Operation(type, accountId, amount, date, categoryId);

            
            Assert.Equal(type, operation.Type);
            Assert.Equal(accountId, operation.BankAccountId);
            Assert.Equal(amount, operation.Amount);
            Assert.Equal(date, operation.Date);
            Assert.Equal(categoryId, operation.CategoryId);
            Assert.Equal("", operation.Description);
            Assert.NotEqual(Guid.Empty, operation.Id);
        }

        [Fact]
        public void Constructor_WithIdAndAllParameters_CreatesOperationWithSpecifiedId()
        {
            
            Guid id = Guid.NewGuid();
            OperationType type = OperationType.Income;
            Guid accountId = Guid.NewGuid();
            decimal amount = 200m;
            DateTime date = DateTime.Now;
            Guid categoryId = Guid.NewGuid();
            string description = "Test description with ID";

            
            var operation = new Operation(id, type, accountId, amount, date, categoryId, description);

            
            Assert.Equal(id, operation.Id);
            Assert.Equal(type, operation.Type);
            Assert.Equal(accountId, operation.BankAccountId);
            Assert.Equal(amount, operation.Amount);
            Assert.Equal(date, operation.Date);
            Assert.Equal(categoryId, operation.CategoryId);
            Assert.Equal(description, operation.Description);
        }

        [Theory]
        [InlineData(OperationType.Income)]
        [InlineData(OperationType.Expense)]
        public void Constructor_WithDifferentOperationTypes_SetsTypeCorrectly(OperationType type)
        {
            
            Guid accountId = Guid.NewGuid();
            decimal amount = 150m;
            DateTime date = DateTime.Now;
            Guid categoryId = Guid.NewGuid();

            
            var operation = new Operation(type, accountId, amount, date, categoryId);

            
            Assert.Equal(type, operation.Type);
        }
    }
}
