using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using FinancialAccounting.Domain;
using FinancialAccounting.Services;
using FinancialAccounting.DataImportExport.DataImport;
using FinancialAccounting.Persistence;

namespace FinancialAccounting.Tests.DataImportExport.DataImport
{
    public class JsonImporterTests
    {
        private readonly string _testDirectory;

        public JsonImporterTests()
        {
            
            _testDirectory = Path.Combine(Path.GetTempPath(), "JsonImporterTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        [Fact]
        public async Task Import_WithValidFile_ImportsAllEntities()
        {
            
            var filePath = Path.Combine(_testDirectory, "test_data.json");
            
            
            var jsonContent = @"{
                ""accounts"": [
                    {
                        ""id"": ""11111111-1111-1111-1111-111111111111"",
                        ""name"": ""Account1"",
                        ""balance"": 1000.50
                    },
                    {
                        ""id"": ""22222222-2222-2222-2222-222222222222"",
                        ""name"": ""Account2"",
                        ""balance"": 2000.75
                    }
                ],
                ""categories"": [
                    {
                        ""id"": ""33333333-3333-3333-3333-333333333333"",
                        ""name"": ""Category1"",
                        ""type"": 0
                    },
                    {
                        ""id"": ""44444444-4444-4444-4444-444444444444"",
                        ""name"": ""Category2"",
                        ""type"": 1
                    }
                ],
                ""operations"": [
                    {
                        ""id"": ""55555555-5555-5555-5555-555555555555"",
                        ""type"": 0,
                        ""bankAccountId"": ""11111111-1111-1111-1111-111111111111"",
                        ""amount"": 500.25,
                        ""date"": ""2023-01-15T10:30:00"",
                        ""categoryId"": ""33333333-3333-3333-3333-333333333333"",
                        ""description"": ""Test income""
                    },
                    {
                        ""id"": ""66666666-6666-6666-6666-666666666666"",
                        ""type"": 1,
                        ""bankAccountId"": ""22222222-2222-2222-2222-222222222222"",
                        ""amount"": 200.75,
                        ""date"": ""2023-01-20T15:45:00"",
                        ""categoryId"": ""44444444-4444-4444-4444-444444444444"",
                        ""description"": ""Test expense""
                    }
                ]
            }";
            
            File.WriteAllText(filePath, jsonContent);
            
            
            var accountRepo = new InMemoryRepository<BankAccount>(a => a.Id);
            var categoryRepo = new InMemoryRepository<Category>(c => c.Id);
            var operationRepo = new InMemoryRepository<Operation>(o => o.Id);
            
            var accountService = new AccountService(accountRepo);
            var categoryService = new CategoryService(categoryRepo);
            var operationService = new OperationService(operationRepo, accountRepo);
            
            var importer = new JsonImporter(accountService, categoryService, operationService);
            
            
            await importer.Import(filePath);
            
            
            var accounts = accountRepo.GetAll();
            var categories = categoryRepo.GetAll();
            var operations = operationRepo.GetAll();
            
            Assert.Equal(2, accounts.Count());
            Assert.Equal(2, categories.Count());
            Assert.Equal(2, operations.Count());
            
            Assert.Contains(accounts, a => a.Name == "Account1" && a.Balance == 1000.50m);
            Assert.Contains(accounts, a => a.Name == "Account2" && a.Balance == 2000.75m);
            
            Assert.Contains(categories, c => c.Name == "Category1" && c.Type == CategoryType.Income);
            Assert.Contains(categories, c => c.Name == "Category2" && c.Type == CategoryType.Expense);
            
            Assert.Contains(operations, o => o.Type == OperationType.Income && o.Amount == 500.25m);
            Assert.Contains(operations, o => o.Type == OperationType.Expense && o.Amount == 200.75m);
        }

        [Fact]
        public async Task Import_WithMissingFile_DoesNotThrowException()
        {
            
            var filePath = Path.Combine(_testDirectory, "missing_data.json");
            
            
            var accountRepo = new InMemoryRepository<BankAccount>(a => a.Id);
            var categoryRepo = new InMemoryRepository<Category>(c => c.Id);
            var operationRepo = new InMemoryRepository<Operation>(o => o.Id);
            
            var accountService = new AccountService(accountRepo);
            var categoryService = new CategoryService(categoryRepo);
            var operationService = new OperationService(operationRepo, accountRepo);
            
            var importer = new JsonImporter(accountService, categoryService, operationService);
            
            
            var exception = await Record.ExceptionAsync(() => importer.Import(filePath));
            Assert.NotNull(exception); 
            
            
            Assert.Empty(accountRepo.GetAll());
            Assert.Empty(categoryRepo.GetAll());
            Assert.Empty(operationRepo.GetAll());
        }

        [Fact]
        public async Task Import_WithInvalidJsonFormat_ThrowsException()
        {
            
            var filePath = Path.Combine(_testDirectory, "invalid_data.json");
            
            
            File.WriteAllText(filePath, "{ invalid json content");
            
            
            var accountRepo = new InMemoryRepository<BankAccount>(a => a.Id);
            var categoryRepo = new InMemoryRepository<Category>(c => c.Id);
            var operationRepo = new InMemoryRepository<Operation>(o => o.Id);
            
            var accountService = new AccountService(accountRepo);
            var categoryService = new CategoryService(categoryRepo);
            var operationService = new OperationService(operationRepo, accountRepo);
            
            var importer = new JsonImporter(accountService, categoryService, operationService);
            
            
            var exception = await Record.ExceptionAsync(() => importer.Import(filePath));
            Assert.NotNull(exception); 
            
            
            Assert.Empty(accountRepo.GetAll());
            Assert.Empty(categoryRepo.GetAll());
            Assert.Empty(operationRepo.GetAll());
        }

        [Fact]
        public async Task Import_WithEmptyJsonFile_ImportsNoEntities()
        {
            
            var filePath = Path.Combine(_testDirectory, "empty_data.json");
            
            
            File.WriteAllText(filePath, "{}");
            
            
            var accountRepo = new InMemoryRepository<BankAccount>(a => a.Id);
            var categoryRepo = new InMemoryRepository<Category>(c => c.Id);
            var operationRepo = new InMemoryRepository<Operation>(o => o.Id);
            
            var accountService = new AccountService(accountRepo);
            var categoryService = new CategoryService(categoryRepo);
            var operationService = new OperationService(operationRepo, accountRepo);
            
            var importer = new JsonImporter(accountService, categoryService, operationService);
            
            
            await importer.Import(filePath);
            
            
            Assert.Empty(accountRepo.GetAll());
            Assert.Empty(categoryRepo.GetAll());
            Assert.Empty(operationRepo.GetAll());
        }

        
        
        [Fact]
        public async Task Import_WithMissingAccountsProperty_ThrowsExceptionForMissingAccount()
        {
            
            var filePath = Path.Combine(_testDirectory, "missing_accounts.json");
            
            
            var jsonContent = @"{
                ""categories"": [
                    {
                        ""id"": ""33333333-3333-3333-3333-333333333333"",
                        ""name"": ""Category1"",
                        ""type"": 0
                    }
                ],
                ""operations"": [
                    {
                        ""id"": ""55555555-5555-5555-5555-555555555555"",
                        ""type"": 0,
                        ""bankAccountId"": ""11111111-1111-1111-1111-111111111111"",
                        ""amount"": 500.25,
                        ""date"": ""2023-01-15T10:30:00"",
                        ""categoryId"": ""33333333-3333-3333-3333-333333333333"",
                        ""description"": ""Test income""
                    }
                ]
            }";
            
            File.WriteAllText(filePath, jsonContent);
            
            
            var accountRepo = new InMemoryRepository<BankAccount>(a => a.Id);
            var categoryRepo = new InMemoryRepository<Category>(c => c.Id);
            var operationRepo = new InMemoryRepository<Operation>(o => o.Id);
            
            var accountService = new AccountService(accountRepo);
            var categoryService = new CategoryService(categoryRepo);
            var operationService = new OperationService(operationRepo, accountRepo);
            
            var importer = new JsonImporter(accountService, categoryService, operationService);
            
            
            var exception = await Record.ExceptionAsync(() => importer.Import(filePath));
            Assert.NotNull(exception); 
            
            
            Assert.Empty(accountRepo.GetAll());
            Assert.Single(categoryRepo.GetAll());
            Assert.Empty(operationRepo.GetAll());
        }

        
        
        [Fact]
        public async Task Import_WithMissingCategoriesProperty_ThrowsExceptionForMissingCategory()
        {
            
            var filePath = Path.Combine(_testDirectory, "missing_categories.json");
            
            
            var jsonContent = @"{
                ""accounts"": [
                    {
                        ""id"": ""11111111-1111-1111-1111-111111111111"",
                        ""name"": ""Account1"",
                        ""balance"": 1000.50
                    }
                ],
                ""operations"": [
                    {
                        ""id"": ""55555555-5555-5555-5555-555555555555"",
                        ""type"": 0,
                        ""bankAccountId"": ""11111111-1111-1111-1111-111111111111"",
                        ""amount"": 500.25,
                        ""date"": ""2023-01-15T10:30:00"",
                        ""categoryId"": ""33333333-3333-3333-3333-333333333333"",
                        ""description"": ""Test income""
                    }
                ]
            }";
            
            File.WriteAllText(filePath, jsonContent);
            
            
            var accountRepo = new InMemoryRepository<BankAccount>(a => a.Id);
            var categoryRepo = new InMemoryRepository<Category>(c => c.Id);
            var operationRepo = new InMemoryRepository<Operation>(o => o.Id);
            
            var accountService = new AccountService(accountRepo);
            var categoryService = new CategoryService(categoryRepo);
            var operationService = new OperationService(operationRepo, accountRepo);
            
            var importer = new JsonImporter(accountService, categoryService, operationService);
            
            
            var exception = await Record.ExceptionAsync(() => importer.Import(filePath));
            Assert.NotNull(exception); 
            
            
            Assert.Single(accountRepo.GetAll());
            Assert.Empty(categoryRepo.GetAll());
            Assert.Empty(operationRepo.GetAll());
        }

        [Fact]
        public async Task Import_WithMissingOperationsProperty_ImportsOnlyAccountsAndCategories()
        {
            
            var filePath = Path.Combine(_testDirectory, "missing_operations.json");
            
            
            var jsonContent = @"{
                ""accounts"": [
                    {
                        ""id"": ""11111111-1111-1111-1111-111111111111"",
                        ""name"": ""Account1"",
                        ""balance"": 1000.50
                    }
                ],
                ""categories"": [
                    {
                        ""id"": ""33333333-3333-3333-3333-333333333333"",
                        ""name"": ""Category1"",
                        ""type"": 0
                    }
                ]
            }";
            
            File.WriteAllText(filePath, jsonContent);
            
            
            var accountRepo = new InMemoryRepository<BankAccount>(a => a.Id);
            var categoryRepo = new InMemoryRepository<Category>(c => c.Id);
            var operationRepo = new InMemoryRepository<Operation>(o => o.Id);
            
            var accountService = new AccountService(accountRepo);
            var categoryService = new CategoryService(categoryRepo);
            var operationService = new OperationService(operationRepo, accountRepo);
            
            var importer = new JsonImporter(accountService, categoryService, operationService);
            
            
            await importer.Import(filePath);
            
            
            Assert.Single(accountRepo.GetAll());
            Assert.Single(categoryRepo.GetAll());
            Assert.Empty(operationRepo.GetAll());
        }

        
        
        [Fact]
        public async Task Import_WithInvalidOperationReferences_ThrowsException()
        {
            
            var filePath = Path.Combine(_testDirectory, "invalid_references.json");
            
            
            var jsonContent = @"{
                ""accounts"": [
                    {
                        ""id"": ""11111111-1111-1111-1111-111111111111"",
                        ""name"": ""Account1"",
                        ""balance"": 1000.50
                    }
                ],
                ""categories"": [
                    {
                        ""id"": ""33333333-3333-3333-3333-333333333333"",
                        ""name"": ""Category1"",
                        ""type"": 0
                    }
                ],
                ""operations"": [
                    {
                        ""id"": ""55555555-5555-5555-5555-555555555555"",
                        ""type"": 0,
                        ""bankAccountId"": ""99999999-9999-9999-9999-999999999999"",
                        ""amount"": 500.25,
                        ""date"": ""2023-01-15T10:30:00"",
                        ""categoryId"": ""33333333-3333-3333-3333-333333333333"",
                        ""description"": ""Test income""
                    }
                ]
            }";
            
            File.WriteAllText(filePath, jsonContent);
            
            
            var accountRepo = new InMemoryRepository<BankAccount>(a => a.Id);
            var categoryRepo = new InMemoryRepository<Category>(c => c.Id);
            var operationRepo = new InMemoryRepository<Operation>(o => o.Id);
            
            var accountService = new AccountService(accountRepo);
            var categoryService = new CategoryService(categoryRepo);
            var operationService = new OperationService(operationRepo, accountRepo);
            
            var importer = new JsonImporter(accountService, categoryService, operationService);
            
            
            var exception = await Record.ExceptionAsync(() => importer.Import(filePath));
            Assert.NotNull(exception); 
            
            
            Assert.Single(accountRepo.GetAll());
            Assert.Single(categoryRepo.GetAll());
            Assert.Empty(operationRepo.GetAll());
        }

        
        
        [Fact]
        public async Task Import_WithInvalidCategoryReference_ThrowsException()
        {
            
            var filePath = Path.Combine(_testDirectory, "invalid_category_ref.json");
            
            
            var jsonContent = @"{
                ""accounts"": [
                    {
                        ""id"": ""11111111-1111-1111-1111-111111111111"",
                        ""name"": ""Account1"",
                        ""balance"": 1000.50
                    }
                ],
                ""categories"": [
                    {
                        ""id"": ""33333333-3333-3333-3333-333333333333"",
                        ""name"": ""Category1"",
                        ""type"": 0
                    }
                ],
                ""operations"": [
                    {
                        ""id"": ""55555555-5555-5555-5555-555555555555"",
                        ""type"": 0,
                        ""bankAccountId"": ""11111111-1111-1111-1111-111111111111"",
                        ""amount"": 500.25,
                        ""date"": ""2023-01-15T10:30:00"",
                        ""categoryId"": ""99999999-9999-9999-9999-999999999999"",
                        ""description"": ""Test income""
                    }
                ]
            }";
            
            File.WriteAllText(filePath, jsonContent);
            
            
            var accountRepo = new InMemoryRepository<BankAccount>(a => a.Id);
            var categoryRepo = new InMemoryRepository<Category>(c => c.Id);
            var operationRepo = new InMemoryRepository<Operation>(o => o.Id);
            
            var accountService = new AccountService(accountRepo);
            var categoryService = new CategoryService(categoryRepo);
            var operationService = new OperationService(operationRepo, accountRepo);
            
            var importer = new JsonImporter(accountService, categoryService, operationService);
            
            
            var exception = await Record.ExceptionAsync(() => importer.Import(filePath));
            Assert.NotNull(exception); 
            
            
            Assert.Single(accountRepo.GetAll());
            Assert.Single(categoryRepo.GetAll());
            Assert.Empty(operationRepo.GetAll());
        }

        [Fact]
        public void Constructor_WithNullParameters_ThrowsArgumentNullException()
        {
            
            var accountRepo = new InMemoryRepository<BankAccount>(a => a.Id);
            var categoryRepo = new InMemoryRepository<Category>(c => c.Id);
            var operationRepo = new InMemoryRepository<Operation>(o => o.Id);
            
            var accountService = new AccountService(accountRepo);
            var categoryService = new CategoryService(categoryRepo);
            var operationService = new OperationService(operationRepo, accountRepo);
            
            
            Assert.Throws<ArgumentNullException>(() => new JsonImporter(null, categoryService, operationService));
            Assert.Throws<ArgumentNullException>(() => new JsonImporter(accountService, null, operationService));
            Assert.Throws<ArgumentNullException>(() => new JsonImporter(accountService, categoryService, null));
        }
    }
}
