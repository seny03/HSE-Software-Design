using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentsService.Data;
using PaymentsService.Models;
using PaymentsService.Services;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace PaymentsService.Tests
{
    public class PaymentsServiceTests
    {
        private readonly PaymentsDbContext _dbContext;
        private readonly Mock<ILogger<PaymentsService.Services.PaymentsService>> _mockLogger;
        private readonly PaymentsService.Services.PaymentsService _paymentsService;

        public PaymentsServiceTests()
        {
            var options = new DbContextOptionsBuilder<PaymentsDbContext>()
                .UseInMemoryDatabase(databaseName: "TestPaymentsDb_" + Guid.NewGuid().ToString())
                .Options;

            _dbContext = new PaymentsDbContext(options);
            _mockLogger = new Mock<ILogger<PaymentsService.Services.PaymentsService>>();
            _paymentsService = new PaymentsService.Services.PaymentsService(_dbContext, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAccountAsync_CreatesAccount()
        {
            
            var userName = "testUser";

            
            var result = await _paymentsService.CreateAccountAsync(userName);

            
            Assert.NotNull(result);
            Assert.Equal(userName, result.UserName);
            Assert.Equal(0, result.Balance);

            
            var account = await _dbContext.Accounts.FindAsync(result.UserId);
            Assert.NotNull(account);
            Assert.Equal(userName, account.UserName);
        }

        [Fact]
        public async Task DepositAsync_IncreasesBalance()
        {
            
            var account = new Account
            {
                UserName = "testUser",
                Balance = 100
            };
            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();

            var depositAmount = 50m;

            
            var result = await _paymentsService.DepositAsync(account.UserId, depositAmount);

            
            Assert.NotNull(result);
            Assert.Equal(150, result.Balance); 

            
            var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => 
                t.AccountId == account.UserId && t.Type == TransactionType.Deposit);
            Assert.NotNull(transaction);
            Assert.Equal(depositAmount, transaction.Amount);
        }

        [Fact]
        public async Task WithdrawAsync_DecreasesBalance_WhenSufficientFunds()
        {
            
            var account = new Account
            {
                UserName = "testUser",
                Balance = 100
            };
            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();

            var withdrawAmount = 30m;

            
            var (success, message) = await _paymentsService.WithdrawAsync(account.UserId, withdrawAmount);

            
            Assert.True(success);
            Assert.Equal("Withdrawal successful", message);

            
            var updatedAccount = await _dbContext.Accounts.FindAsync(account.UserId);
            Assert.NotNull(updatedAccount);
            Assert.Equal(70, updatedAccount.Balance); 

            
            var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => 
                t.AccountId == account.UserId && t.Type == TransactionType.Withdrawal);
            Assert.NotNull(transaction);
            Assert.Equal(-withdrawAmount, transaction.Amount);
        }

        [Fact]
        public async Task WithdrawAsync_FailsWithInsufficientFunds_WhenInsufficientFunds()
        {
            
            var account = new Account
            {
                UserName = "testUser",
                Balance = 20
            };
            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();

            var withdrawAmount = 50m;

            
            var (success, message) = await _paymentsService.WithdrawAsync(account.UserId, withdrawAmount);

            
            Assert.False(success);
            Assert.Equal("Insufficient funds", message);

            
            var updatedAccount = await _dbContext.Accounts.FindAsync(account.UserId);
            Assert.NotNull(updatedAccount);
            Assert.Equal(20, updatedAccount.Balance);
        }

        [Fact]
        public async Task GetBalanceAsync_ReturnsBalance()
        {
            
            var account = new Account
            {
                UserName = "testUser",
                Balance = 150
            };
            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();

            
            var result = await _paymentsService.GetBalanceAsync(account.UserId);

            
            Assert.NotNull(result);
            Assert.Equal(150, result);
        }

        [Fact]
        public async Task GetAllAccountsAsync_ReturnsAllAccounts()
        {
            
            _dbContext.Accounts.AddRange(
                new Account { UserName = "user1", Balance = 100 },
                new Account { UserName = "user2", Balance = 200 },
                new Account { UserName = "user3", Balance = 300 }
            );
            await _dbContext.SaveChangesAsync();

            
            var result = await _paymentsService.GetAllAccountsAsync();

            
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, a => a.UserName == "user1" && a.Balance == 100);
            Assert.Contains(result, a => a.UserName == "user2" && a.Balance == 200);
            Assert.Contains(result, a => a.UserName == "user3" && a.Balance == 300);
        }
    }
} 