using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Moq;
using FinancialAccounting.Domain;
using FinancialAccounting.Persistence;
using FinancialAccounting.DataImportExport.DataExport;

namespace FinancialAccounting.Tests.DataImportExport.DataExport
{
    public class CsvExportVisitorTests
    {
        private readonly Mock<IRepository<BankAccount>> _mockAccountRepo;
        private readonly Mock<IRepository<Category>> _mockCategoryRepo;
        private readonly Mock<IRepository<Operation>> _mockOperationRepo;
        private readonly string _testDirectory;

        public CsvExportVisitorTests()
        {
            _mockAccountRepo = new Mock<IRepository<BankAccount>>();
            _mockCategoryRepo = new Mock<IRepository<Category>>();
            _mockOperationRepo = new Mock<IRepository<Operation>>();
            
            
            _testDirectory = Path.Combine(Path.GetTempPath(), "CsvExportVisitorTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        [Fact]
        public void Visit_AddsEntitiesToLists()
        {
            
            var visitor = new CsvExportVisitor();
            var account = new BankAccount("TestAccount", 1000m) { Id = Guid.NewGuid() };
            var category = new Category("TestCategory", CategoryType.Income) { Id = Guid.NewGuid() };
            var operation = new Operation(
                OperationType.Income, 
                account.Id, 
                500m, 
                DateTime.Now, 
                category.Id, 
                "Test operation") 
            { 
                Id = Guid.NewGuid() 
            };

            
            visitor.Visit(account);
            visitor.Visit(category);
            visitor.Visit(operation);

            
            var exportPath = Path.Combine(_testDirectory, "test_export");
            
            
            _mockAccountRepo.Setup(r => r.GetAll()).Returns(new List<BankAccount>());
            _mockCategoryRepo.Setup(r => r.GetAll()).Returns(new List<Category>());
            _mockOperationRepo.Setup(r => r.GetAll()).Returns(new List<Operation>());
            
            
            visitor.ExportToFile(exportPath, _mockAccountRepo.Object, _mockCategoryRepo.Object, _mockOperationRepo.Object);
            
            
            Assert.True(File.Exists(exportPath + "_accounts.csv"));
            Assert.True(File.Exists(exportPath + "_categories.csv"));
            Assert.True(File.Exists(exportPath + "_operations.csv"));
            
            
            var accountsContent = File.ReadAllText(exportPath + "_accounts.csv");
            var categoriesContent = File.ReadAllText(exportPath + "_categories.csv");
            var operationsContent = File.ReadAllText(exportPath + "_operations.csv");
            
            Assert.Contains(account.Id.ToString(), accountsContent);
            Assert.Contains(account.Name, accountsContent);
            
            Assert.Contains(category.Id.ToString(), categoriesContent);
            Assert.Contains(category.Name, categoriesContent);
            
            Assert.Contains(operation.Id.ToString(), operationsContent);
            Assert.Contains(operation.Description, operationsContent);
        }

        [Fact]
        public void ExportToFile_WithRepositoryEntities_ExportsAllEntities()
        {
            
            var visitor = new CsvExportVisitor();
            var accounts = new List<BankAccount>
            {
                new BankAccount("Account1", 1000m) { Id = Guid.NewGuid() },
                new BankAccount("Account2", 2000m) { Id = Guid.NewGuid() }
            };
            
            var categories = new List<Category>
            {
                new Category("Category1", CategoryType.Income) { Id = Guid.NewGuid() },
                new Category("Category2", CategoryType.Expense) { Id = Guid.NewGuid() }
            };
            
            var operations = new List<Operation>
            {
                new Operation(OperationType.Income, accounts[0].Id, 500m, DateTime.Now, categories[0].Id, "Operation1") { Id = Guid.NewGuid() },
                new Operation(OperationType.Expense, accounts[1].Id, 300m, DateTime.Now, categories[1].Id, "Operation2") { Id = Guid.NewGuid() }
            };
            
            _mockAccountRepo.Setup(r => r.GetAll()).Returns(accounts);
            _mockCategoryRepo.Setup(r => r.GetAll()).Returns(categories);
            _mockOperationRepo.Setup(r => r.GetAll()).Returns(operations);
            
            var exportPath = Path.Combine(_testDirectory, "repo_export");
            
            
            visitor.ExportToFile(exportPath, _mockAccountRepo.Object, _mockCategoryRepo.Object, _mockOperationRepo.Object);
            
            
            Assert.True(File.Exists(exportPath + "_accounts.csv"));
            Assert.True(File.Exists(exportPath + "_categories.csv"));
            Assert.True(File.Exists(exportPath + "_operations.csv"));
            
            
            var accountsContent = File.ReadAllText(exportPath + "_accounts.csv");
            var categoriesContent = File.ReadAllText(exportPath + "_categories.csv");
            var operationsContent = File.ReadAllText(exportPath + "_operations.csv");
            
            foreach (var account in accounts)
            {
                Assert.Contains(account.Id.ToString(), accountsContent);
                Assert.Contains(account.Name, accountsContent);
            }
            
            foreach (var category in categories)
            {
                Assert.Contains(category.Id.ToString(), categoriesContent);
                Assert.Contains(category.Name, categoriesContent);
            }
            
            foreach (var operation in operations)
            {
                Assert.Contains(operation.Id.ToString(), operationsContent);
                Assert.Contains(operation.Description, operationsContent);
            }
        }
    }
}
