using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FinancialAccounting.Services;

namespace FinancialAccounting.Tests.Commands
{
    public class ReportBalanceCommandTests
    {
        [Fact]
        public async Task ExecuteAsync_CallsOperationServiceBalanceDifference()
        {
            
            var mockOperationRepo = new Mock<FinancialAccounting.Persistence.IRepository<FinancialAccounting.Domain.Operation>>();
            var mockAccountRepo = new Mock<FinancialAccounting.Persistence.IRepository<FinancialAccounting.Domain.BankAccount>>();
            
            
            var accountId = Guid.NewGuid();
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 1, 31);
            
            var operations = new List<FinancialAccounting.Domain.Operation>
            {
                new FinancialAccounting.Domain.Operation(
                    FinancialAccounting.Domain.OperationType.Income, 
                    accountId, 
                    500m, 
                    new DateTime(2023, 1, 15), 
                    Guid.NewGuid())
            };
            
            mockOperationRepo.Setup(r => r.GetAll()).Returns(operations);
            
            var operationService = new FinancialAccounting.Services.OperationService(mockOperationRepo.Object, mockAccountRepo.Object);
            var command = new ReportBalanceCommand(operationService, accountId, startDate, endDate);
            
            
            await command.ExecuteAsync();
            
            
            mockOperationRepo.Verify(r => r.GetAll(), Times.Once);
        }
    }
}
