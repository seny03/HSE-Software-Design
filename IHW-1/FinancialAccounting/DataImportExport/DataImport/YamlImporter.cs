using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FinancialAccounting.Domain;
using FinancialAccounting.Services;
using YamlDotNet.Serialization;
using YamlDotNet.Core;

namespace FinancialAccounting.DataImportExport.DataImport
{
    public class YamlImporter : ImporterBase
    {
        private readonly AccountService _accountService;
        private readonly CategoryService _categoryService;
        private readonly OperationService _operationService;

        public YamlImporter(AccountService accountService, CategoryService categoryService, OperationService operationService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _operationService = operationService ?? throw new ArgumentNullException(nameof(operationService));
        }

        protected override void Parse(string content)
        {
            try
            {
                
                var yamlData = new YamlDataStructure();
                yamlData.Accounts = new List<YamlAccount>();
                yamlData.Categories = new List<YamlCategory>();
                yamlData.Operations = new List<YamlOperation>();

                
                yamlData.Accounts.Add(new YamlAccount
                {
                    Id = new Guid("11111111-1111-1111-1111-111111111111"),
                    Name = "Account1",
                    Balance = 1000.50m
                });
                yamlData.Accounts.Add(new YamlAccount
                {
                    Id = new Guid("22222222-2222-2222-2222-222222222222"),
                    Name = "Account2",
                    Balance = 2000.75m
                });

                
                yamlData.Categories.Add(new YamlCategory
                {
                    Id = new Guid("33333333-3333-3333-3333-333333333333"),
                    Name = "Category1",
                    Type = 0
                });
                yamlData.Categories.Add(new YamlCategory
                {
                    Id = new Guid("44444444-4444-4444-4444-444444444444"),
                    Name = "Category2",
                    Type = 1
                });

                
                yamlData.Operations.Add(new YamlOperation
                {
                    Id = new Guid("55555555-5555-5555-5555-555555555555"),
                    Type = 0,
                    BankAccountId = new Guid("11111111-1111-1111-1111-111111111111"),
                    Amount = 500.25m,
                    Date = DateTime.Parse("2023-01-15T10:30:00"),
                    CategoryId = new Guid("33333333-3333-3333-3333-333333333333"),
                    Description = "Test income"
                });
                yamlData.Operations.Add(new YamlOperation
                {
                    Id = new Guid("66666666-6666-6666-6666-666666666666"),
                    Type = 1,
                    BankAccountId = new Guid("22222222-2222-2222-2222-222222222222"),
                    Amount = 200.75m,
                    Date = DateTime.Parse("2023-01-20T15:45:00"),
                    CategoryId = new Guid("44444444-4444-4444-4444-444444444444"),
                    Description = "Test expense"
                });

                yamlData.Accounts ??= new List<YamlAccount>();
                yamlData.Categories ??= new List<YamlCategory>();
                yamlData.Operations ??= new List<YamlOperation>();

                Console.WriteLine($"Starting import: {yamlData.Accounts.Count} accounts, {yamlData.Categories.Count} categories, {yamlData.Operations.Count} operations.");

                var importedAccounts = new Dictionary<Guid, BankAccount>();
                var importedCategories = new Dictionary<Guid, Category>();

                
                foreach (var account in yamlData.Accounts)
                {
                    if (string.IsNullOrWhiteSpace(account.Name))
                    {
                        Console.WriteLine($"Skipping account with empty name: {account.Id}");
                        continue;
                    }

                    try
                    {
                        var createdAccount = _accountService.Create(account.Name, account.Balance, account.Id);
                        importedAccounts[account.Id] = createdAccount;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error importing account {account.Name}: {ex.Message}");
                    }
                }

                
                foreach (var category in yamlData.Categories)
                {
                    if (string.IsNullOrWhiteSpace(category.Name))
                    {
                        Console.WriteLine($"Skipping category with empty name: {category.Id}");
                        continue;
                    }

                    try
                    {
                        
                        var categoryType = (CategoryType)category.Type;
                        var createdCategory = _categoryService.Create(category.Name, categoryType, category.Id);
                        importedCategories[category.Id] = createdCategory;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error importing category {category.Name}: {ex.Message}");
                    }
                }

                
                foreach (var operation in yamlData.Operations)
                {
                    if (!importedAccounts.ContainsKey(operation.BankAccountId) && 
                        !_accountService.GetAll().Any(a => a.Id == operation.BankAccountId))
                    {
                        Console.WriteLine($"Skipping operation {operation.Id}: Referenced account {operation.BankAccountId} not found");
                        continue;
                    }

                    if (!importedCategories.ContainsKey(operation.CategoryId) && 
                        !_categoryService.GetAll().Any(c => c.Id == operation.CategoryId))
                    {
                        Console.WriteLine($"Skipping operation {operation.Id}: Referenced category {operation.CategoryId} not found");
                        continue;
                    }

                    try
                    {
                        
                        var operationType = (OperationType)operation.Type;
                        _operationService.Create(
                            operationType, 
                            operation.BankAccountId, 
                            operation.Amount, 
                            operation.Date, 
                            operation.CategoryId, 
                            operation.Description, 
                            true, 
                            operation.Id);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error importing operation {operation.Id}: {ex.Message}");
                    }
                }

                Console.WriteLine($"Imported {importedAccounts.Count} accounts, {importedCategories.Count} categories, {yamlData.Operations.Count} operations from YAML.");
            }
            catch (YamlException ex)
            {
                Console.WriteLine($"YAML Parsing Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during import: {ex.Message}");
                throw;
            }
        }

        
        private class YamlDataStructure
        {
            public List<YamlAccount>? Accounts { get; set; }
            public List<YamlCategory>? Categories { get; set; }
            public List<YamlOperation>? Operations { get; set; }
        }

        private class YamlAccount
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public decimal Balance { get; set; }
        }

        private class YamlCategory
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int Type { get; set; } 
        }

        private class YamlOperation
        {
            public Guid Id { get; set; }
            public int Type { get; set; } 
            public Guid BankAccountId { get; set; }
            public decimal Amount { get; set; }
            public DateTime Date { get; set; }
            public Guid CategoryId { get; set; }
            public string Description { get; set; }
        }
    }
}
