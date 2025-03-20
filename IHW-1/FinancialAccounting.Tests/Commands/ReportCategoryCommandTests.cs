using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FinancialAccounting.Services;

namespace FinancialAccounting.Tests.Commands
{
    public class ReportCategoryCommandTests
    {
        [Fact]
        public async Task ExecuteAsync_CallsOperationServiceGroupByCategory()
        {
            
            var mockOperationRepo = new Mock<FinancialAccounting.Persistence.IRepository<FinancialAccounting.Domain.Operation>>();
            var mockAccountRepo = new Mock<FinancialAccounting.Persistence.IRepository<FinancialAccounting.Domain.BankAccount>>();
            var mockCategoryRepo = new Mock<FinancialAccounting.Persistence.IRepository<FinancialAccounting.Domain.Category>>();
            
            var accountId = Guid.NewGuid();
            var category1Id = Guid.NewGuid();
            var category2Id = Guid.NewGuid();
            
            
            var categories = new List<FinancialAccounting.Domain.Category>
            {
                new FinancialAccounting.Domain.Category(category1Id, "Category1", FinancialAccounting.Domain.CategoryType.Income),
                new FinancialAccounting.Domain.Category(category2Id, "Category2", FinancialAccounting.Domain.CategoryType.Expense)
            };
            
            
            var operations = new List<FinancialAccounting.Domain.Operation>
            {
                new FinancialAccounting.Domain.Operation(
                    FinancialAccounting.Domain.OperationType.Income, 
                    accountId, 
                    500m, 
                    DateTime.Now, 
                    category1Id),
                new FinancialAccounting.Domain.Operation(
                    FinancialAccounting.Domain.OperationType.Expense, 
                    accountId, 
                    200m, 
                    DateTime.Now, 
                    category2Id)
            };
            
            mockOperationRepo.Setup(r => r.GetAll()).Returns(operations);
            mockCategoryRepo.Setup(r => r.GetAll()).Returns(categories);
            
            var operationService = new FinancialAccounting.Services.OperationService(mockOperationRepo.Object, mockAccountRepo.Object);
            var categoryService = new FinancialAccounting.Services.CategoryService(mockCategoryRepo.Object);
            
            var command = new ReportCategoryCommand(operationService, categoryService, accountId);
            
            
            await command.ExecuteAsync();
            
            
            mockOperationRepo.Verify(r => r.GetAll(), Times.Once);
            mockCategoryRepo.Verify(r => r.GetAll(), Times.AtLeastOnce);
        }
    }
}
