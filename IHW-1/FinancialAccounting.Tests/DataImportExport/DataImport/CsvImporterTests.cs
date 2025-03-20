using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FinancialAccounting.Domain;
using FinancialAccounting.Services;
using FinancialAccounting.DataImportExport.DataImport;

namespace FinancialAccounting.Tests.DataImportExport.DataImport
{
    public class CsvImporterTests
    {
        private readonly Mock<AccountService> _mockAccountService;
        private readonly Mock<CategoryService> _mockCategoryService;
        private readonly Mock<OperationService> _mockOperationService;
        private readonly string _testDirectory;

        public CsvImporterTests()
        {
            _mockAccountService = new Mock<AccountService>(Mock.Of<FinancialAccounting.Persistence.IRepository<BankAccount>>());
            _mockCategoryService = new Mock<CategoryService>(Mock.Of<FinancialAccounting.Persistence.IRepository<Category>>());
            _mockOperationService = new Mock<OperationService>(
                Mock.Of<FinancialAccounting.Persistence.IRepository<Operation>>(),
                Mock.Of<FinancialAccounting.Persistence.IRepository<BankAccount>>());
            
            
            _testDirectory = Path.Combine(Path.GetTempPath(), "CsvImporterTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        [Fact]
        public async Task Import_WithValidFiles_ImportsAllEntities()
        {
            
            var baseFilePath = Path.Combine(_testDirectory, "test_data");
            var accountsFilePath = baseFilePath + "_accounts.csv";
            var categoriesFilePath = baseFilePath + "_categories.csv";
            var operationsFilePath = baseFilePath + "_operations.csv";
            
            
            File.WriteAllText(accountsFilePath, 
                "Id,Name,Balance\n" +
                "11111111-1111-1111-1111-111111111111,Account1,1000.50\n" +
                "22222222-2222-2222-2222-222222222222,Account2,2000.75");
            
            File.WriteAllText(categoriesFilePath, 
                "Id,Name,Type\n" +
                "33333333-3333-3333-3333-333333333333,Category1,Income\n" +
                "44444444-4444-4444-4444-444444444444,Category2,Expense");
            
            File.WriteAllText(operationsFilePath, 
                "Id,Type,BankAccountId,Amount,Date,CategoryId,Description\n" +
                "55555555-5555-5555-5555-555555555555,Income,11111111-1111-1111-1111-111111111111,500.25,2023-01-15T10:30:00,33333333-3333-3333-3333-333333333333,Test income\n" +
                "66666666-6666-6666-6666-666666666666,Expense,22222222-2222-2222-2222-222222222222,200.75,2023-01-20T15:45:00,44444444-4444-4444-4444-444444444444,Test expense");
            
            
            var accountRepo = new FinancialAccounting.Persistence.InMemoryRepository<FinancialAccounting.Domain.BankAccount>(a => a.Id);
            var categoryRepo = new FinancialAccounting.Persistence.InMemoryRepository<FinancialAccounting.Domain.Category>(c => c.Id);
            var operationRepo = new FinancialAccounting.Persistence.InMemoryRepository<FinancialAccounting.Domain.Operation>(o => o.Id);
            
            var accountService = new FinancialAccounting.Services.AccountService(accountRepo);
            var categoryService = new FinancialAccounting.Services.CategoryService(categoryRepo);
            var operationService = new FinancialAccounting.Services.OperationService(operationRepo, accountRepo);
            
            
            var account1 = new FinancialAccounting.Domain.BankAccount("Account1", 1000.50m) { Id = new Guid("11111111-1111-1111-1111-111111111111") };
            var account2 = new FinancialAccounting.Domain.BankAccount("Account2", 2000.75m) { Id = new Guid("22222222-2222-2222-2222-222222222222") };
            accountRepo.Add(account1);
            accountRepo.Add(account2);
            
            var importer = new CsvImporter(accountService, categoryService, operationService);
            
            
            await importer.Import(baseFilePath);
            
            
            var categories = categoryRepo.GetAll().ToList();
            var operations = operationRepo.GetAll().ToList();
            
            Assert.Equal(2, categories.Count);
            Assert.Equal(2, operations.Count);
            
            Assert.Contains(categories, c => c.Name == "Category1" && c.Type == CategoryType.Income);
            Assert.Contains(categories, c => c.Name == "Category2" && c.Type == CategoryType.Expense);
            
            Assert.Contains(operations, o => o.Type == OperationType.Income && o.Amount == 500.25m);
            Assert.Contains(operations, o => o.Type == OperationType.Expense && o.Amount == 200.75m);
        }

        [Fact]
        public async Task Import_WithMissingFiles_DoesNotThrowException()
        {
            
            var baseFilePath = Path.Combine(_testDirectory, "missing_data");
            
            
            var accountRepo = new FinancialAccounting.Persistence.InMemoryRepository<FinancialAccounting.Domain.BankAccount>(a => a.Id);
            var categoryRepo = new FinancialAccounting.Persistence.InMemoryRepository<FinancialAccounting.Domain.Category>(c => c.Id);
            var operationRepo = new FinancialAccounting.Persistence.InMemoryRepository<FinancialAccounting.Domain.Operation>(o => o.Id);
            
            var accountService = new FinancialAccounting.Services.AccountService(accountRepo);
            var categoryService = new FinancialAccounting.Services.CategoryService(categoryRepo);
            var operationService = new FinancialAccounting.Services.OperationService(operationRepo, accountRepo);
            
            var importer = new CsvImporter(accountService, categoryService, operationService);
            
            
            var exception = await Record.ExceptionAsync(() => importer.Import(baseFilePath));
            Assert.Null(exception);
            
            
            Assert.Empty(accountRepo.GetAll());
            Assert.Empty(categoryRepo.GetAll());
            Assert.Empty(operationRepo.GetAll());
        }

        [Fact]
        public async Task Import_WithInvalidCsvFormat_DoesNotThrowException()
        {
            
            var baseFilePath = Path.Combine(_testDirectory, "invalid_data");
            var accountsFilePath = baseFilePath + "_accounts.csv";
            
            
            File.WriteAllText(accountsFilePath, 
                "Name,Balance\n" +  
                "Account1,1000.50");
            
            
            var accountRepo = new FinancialAccounting.Persistence.InMemoryRepository<FinancialAccounting.Domain.BankAccount>(a => a.Id);
            var accountService = new FinancialAccounting.Services.AccountService(accountRepo);
            
            var importer = new CsvImporter(accountService);
            
            
            var exception = await Record.ExceptionAsync(() => importer.Import(baseFilePath));
            Assert.Null(exception);
            
            
            Assert.Empty(accountRepo.GetAll());
        }
    }
}
