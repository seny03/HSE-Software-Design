using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FinancialAccounting.Domain;
using FinancialAccounting.Services;

namespace FinancialAccounting.DataImportExport.DataImport
{
    public class CsvImporter : ImporterBase
    {
        private readonly AccountService _accountService;
        private readonly CategoryService _categoryService;
        private readonly OperationService _operationService;

        public CsvImporter(AccountService accountService, CategoryService? categoryService = null, OperationService? operationService = null)
        {
            _accountService = accountService;
            _categoryService = categoryService;
            _operationService = operationService;
        }

        public override async Task Import(string path)
        {
            try
            {
                
                string baseFilePath = Path.Combine(
                    Path.GetDirectoryName(path) ?? "",
                    Path.GetFileNameWithoutExtension(path)
                );
                
                
                string accountsFilePath = $"{baseFilePath}_accounts.csv";
                string categoriesFilePath = $"{baseFilePath}_categories.csv";
                string operationsFilePath = $"{baseFilePath}_operations.csv";

                int accountsImported = 0;
                int categoriesImported = 0;
                int operationsImported = 0;

                
                if (File.Exists(accountsFilePath))
                {
                    var lines = await File.ReadAllLinesAsync(accountsFilePath);
                    if (lines.Length > 1) 
                    {
                        
                        for (int i = 1; i < lines.Length; i++)
                        {
                            var line = lines[i];
                            if (string.IsNullOrWhiteSpace(line)) continue;

                            var values = line.Split(',');
                            if (values.Length >= 3)
                            {
                                var id = Guid.Parse(values[0]);
                                var name = values[1];
                                var balance = decimal.Parse(values[2], CultureInfo.InvariantCulture);

                                _accountService.Create(name, balance, id);
                                accountsImported++;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Warning: Accounts file not found at {accountsFilePath}");
                }

                
                if (File.Exists(categoriesFilePath) && _categoryService != null)
                {
                    var lines = await File.ReadAllLinesAsync(categoriesFilePath);
                    if (lines.Length > 1) 
                    {
                        
                        for (int i = 1; i < lines.Length; i++)
                        {
                            var line = lines[i];
                            if (string.IsNullOrWhiteSpace(line)) continue;

                            var values = line.Split(',');
                            if (values.Length >= 3)
                            {
                                var id = Guid.Parse(values[0]);
                                var name = values[1];
                                var type = Enum.Parse<CategoryType>(values[2]);

                                _categoryService.Create(name, type, id);
                                categoriesImported++;
                            }
                        }
                    }
                }
                else if (_categoryService != null)
                {
                    Console.WriteLine($"Warning: Categories file not found at {categoriesFilePath}");
                }

                
                if (File.Exists(operationsFilePath) && _operationService != null)
                {
                    var lines = await File.ReadAllLinesAsync(operationsFilePath);
                    if (lines.Length > 1) 
                    {
                        
                        for (int i = 1; i < lines.Length; i++)
                        {
                            var line = lines[i];
                            if (string.IsNullOrWhiteSpace(line)) continue;

                            var values = line.Split(',');
                            if (values.Length >= 6)
                            {
                                var id = Guid.Parse(values[0]);
                                var type = Enum.Parse<OperationType>(values[1]);
                                var accountId = Guid.Parse(values[2]);
                                var amount = decimal.Parse(values[3], CultureInfo.InvariantCulture);
                                var date = DateTime.Parse(values[4], CultureInfo.InvariantCulture);
                                var categoryId = Guid.Parse(values[5]);
                                var description = values.Length > 6 ? values[6] : "";

                                _operationService.Create(
                                    type, 
                                    accountId, 
                                    amount, 
                                    date, 
                                    categoryId, 
                                    description, 
                                    true, 
                                    id);
                                operationsImported++;
                            }
                        }
                    }
                }
                else if (_operationService != null)
                {
                    Console.WriteLine($"Warning: Operations file not found at {operationsFilePath}");
                }

                Console.WriteLine($"Imported {accountsImported} accounts, {categoriesImported} categories, {operationsImported} operations from CSV.");
                
                
                if (accountsImported == 0 && categoriesImported == 0 && operationsImported == 0)
                {
                    Console.WriteLine($"No data was imported. Expected files with pattern: {baseFilePath}_[accounts|categories|operations].csv");
                }
                
                await Task.CompletedTask; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during CSV import: {ex.Message}");
                throw;
            }
        }

        protected override void Parse(string content)
        {
            
            throw new NotImplementedException("This method is not used for CSV import. Use Import method instead.");
        }
    }
}
