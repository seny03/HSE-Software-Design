using System;
using Xunit;
using FinancialAccounting.Domain;

namespace FinancialAccounting.Tests.Domain
{
    public class BankAccountTests
    {
        [Fact]
        public void Constructor_WithValidParameters_CreatesAccount()
        {
            
            string name = "TestAccount";
            decimal balance = 1000m;

            
            var account = new BankAccount(name, balance);

            
            Assert.Equal(name, account.Name);
            Assert.Equal(balance, account.Balance);
            Assert.NotEqual(Guid.Empty, account.Id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidName_ThrowsArgumentException(string invalidName)
        {
            
            decimal balance = 1000m;

            
            Assert.Throws<ArgumentException>(() => new BankAccount(invalidName, balance));
        }

        [Fact]
        public void Constructor_WithNegativeBalance_ThrowsArgumentException()
        {
            
            string name = "TestAccount";
            decimal negativeBalance = -100m;

            
            Assert.Throws<ArgumentException>(() => new BankAccount(name, negativeBalance));
        }

        [Fact]
        public void Apply_IncomeOperation_IncreasesBalance()
        {
            
            var account = new BankAccount("TestAccount", 1000m);
            decimal amount = 500m;
            decimal expectedBalance = 1500m;

            
            account.Apply(OperationType.Income, amount);

            
            Assert.Equal(expectedBalance, account.Balance);
        }

        [Fact]
        public void Apply_ExpenseOperation_DecreasesBalance()
        {
            
            var account = new BankAccount("TestAccount", 1000m);
            decimal amount = 300m;
            decimal expectedBalance = 700m;

            
            account.Apply(OperationType.Expense, amount);

            
            Assert.Equal(expectedBalance, account.Balance);
        }
    }
}
