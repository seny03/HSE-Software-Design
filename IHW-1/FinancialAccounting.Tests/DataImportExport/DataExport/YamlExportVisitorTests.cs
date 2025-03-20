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
    public class YamlExportVisitorTests
    {
        private readonly Mock<IRepository<BankAccount>> _mockAccountRepo;
        private readonly Mock<IRepository<Category>> _mockCategoryRepo;
        private readonly Mock<IRepository<Operation>> _mockOperationRepo;
        private readonly string _testDirectory;

        public YamlExportVisitorTests()
        {
            _mockAccountRepo = new Mock<IRepository<BankAccount>>();
            _mockCategoryRepo = new Mock<IRepository<Category>>();
            _mockOperationRepo = new Mock<IRepository<Operation>>();

            
            _testDirectory = Path.Combine(Path.GetTempPath(), "YamlExportVisitorTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        [Fact]
        public void Visit_AddsEntitiesToLists()
        {
            
            var visitor = new YamlExportVisitor();
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

            
            var exportPath = Path.Combine(_testDirectory, "test_export.yaml");

            
            _mockAccountRepo.Setup(r => r.GetAll()).Returns(new List<BankAccount> { account });
            _mockCategoryRepo.Setup(r => r.GetAll()).Returns(new List<Category> { category });
            _mockOperationRepo.Setup(r => r.GetAll()).Returns(new List<Operation> { operation });

            
            visitor.ExportToFile(exportPath, _mockAccountRepo.Object, _mockCategoryRepo.Object, _mockOperationRepo.Object);

            
            Assert.True(File.Exists(exportPath));

            
            var content = File.ReadAllText(exportPath);

            Assert.Contains(account.Id.ToString(), content);
            Assert.Contains(account.Name, content);
            Assert.Contains(category.Id.ToString(), content);
            Assert.Contains(category.Name, content);
            Assert.Contains(operation.Id.ToString(), content);
            Assert.Contains(operation.Description, content);
        }

        [Fact]
        public void ExportToFile_WithRepositoryEntities_ExportsAllEntities()
        {
            
            var visitor = new YamlExportVisitor();
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

            var exportPath = Path.Combine(_testDirectory, "repo_export.yaml");

            
            visitor.ExportToFile(exportPath, _mockAccountRepo.Object, _mockCategoryRepo.Object, _mockOperationRepo.Object);

            
            Assert.True(File.Exists(exportPath));

            
            var content = File.ReadAllText(exportPath);

            foreach (var account in accounts)
            {
                Assert.Contains(account.Id.ToString(), content);
                Assert.Contains(account.Name, content);
            }

            foreach (var category in categories)
            {
                Assert.Contains(category.Id.ToString(), content);
                Assert.Contains(category.Name, content);
            }

            foreach (var operation in operations)
            {
                Assert.Contains(operation.Id.ToString(), content);
                Assert.Contains(operation.Description, content);
            }
        }
    }
}
