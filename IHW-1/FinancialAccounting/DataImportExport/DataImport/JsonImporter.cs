using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FinancialAccounting.Domain;
using FinancialAccounting.Services;

namespace FinancialAccounting.DataImportExport.DataImport
{
    public class JsonImporter : ImporterBase
    {
        private readonly AccountService _accountService;
        private readonly CategoryService _categoryService;
        private readonly OperationService _operationService;

        public JsonImporter(AccountService accountService, CategoryService categoryService, OperationService operationService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _operationService = operationService ?? throw new ArgumentNullException(nameof(operationService));
        }

        protected override void Parse(string content)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                };
                
                var jsonData = JsonSerializer.Deserialize<JsonDataStructure>(content, options)
                    ?? throw new InvalidOperationException("Failed to deserialize JSON file. The content might be invalid.");

                jsonData.Accounts ??= new List<BankAccount>();
                jsonData.Categories ??= new List<Category>();
                jsonData.Operations ??= new List<Operation>();

                Console.WriteLine($"Starting import: {jsonData.Accounts.Count} accounts, {jsonData.Categories.Count} categories, {jsonData.Operations.Count} operations.");

                var importedAccounts = new Dictionary<Guid, BankAccount>();
                var importedCategories = new Dictionary<Guid, Category>();

                
                foreach (var account in jsonData.Accounts)
                {
                    var createdAccount = _accountService.Create(account.Name, account.Balance, account.Id);
                    importedAccounts[account.Id] = createdAccount;
                }

                
                foreach (var category in jsonData.Categories)
                {
                    var createdCategory = _categoryService.Create(category.Name, category.Type, category.Id);
                    importedCategories[category.Id] = createdCategory;
                }

                
                foreach (var operation in jsonData.Operations)
                {
                    if (!importedAccounts.ContainsKey(operation.BankAccountId))
                    {
                        throw new InvalidOperationException($"Operation references a missing account ID: {operation.BankAccountId}");
                    }

                    if (!importedCategories.ContainsKey(operation.CategoryId))
                    {
                        throw new InvalidOperationException($"Operation references a missing category ID: {operation.CategoryId}");
                    }

                    _operationService.Create(operation.Type, operation.BankAccountId, operation.Amount, operation.Date, operation.CategoryId, operation.Description, true, operation.Id);
                }

                Console.WriteLine($"Imported {jsonData.Accounts.Count} accounts, {jsonData.Categories.Count} categories, {jsonData.Operations.Count} operations from JSON.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during import: {ex.Message}");
                throw;
            }
        }



        private class JsonDataStructure
        {
            public List<BankAccount>? Accounts { get; set; }
            public List<Category>? Categories { get; set; }
            public List<Operation>? Operations { get; set; }
        }
    }
}
