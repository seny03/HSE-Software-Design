﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using FinancialAccounting.Persistence;
using FinancialAccounting.Domain;
using FinancialAccounting.Services;
using FinancialAccounting.DataImportExport.DataImport;
using FinancialAccounting.DataImportExport.DataExport;

var services = new ServiceCollection();


services.AddSingleton<IRepository<BankAccount>>(sp => new InMemoryRepository<BankAccount>(a => a.Id));
services.AddSingleton<IRepository<Category>>(sp => new InMemoryRepository<Category>(c => c.Id));
services.AddSingleton<IRepository<Operation>>(sp => new InMemoryRepository<Operation>(o => o.Id));


services.AddSingleton<AccountService>();
services.AddSingleton<CategoryService>();
services.AddSingleton<OperationService>();

var sp = services.BuildServiceProvider();
var accSvc = sp.GetRequiredService<AccountService>();
var catSvc = sp.GetRequiredService<CategoryService>();
var opSvc = sp.GetRequiredService<OperationService>();


bool timeNextCommand = false;

Console.WriteLine("CLI Ready");

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine()?.Split(' ');

    if (input == null || input.Length == 0) continue;

    var command = input[0].Trim().ToLower();

    try
    {
        switch (command)
        {
            case "exit":
                return;
                
            case "time":
                timeNextCommand = true;
                Console.WriteLine("Next command will be timed");
                break;

            case "add-account":
                {
                    if (input.Length != 3)
                    {
                        throw new ArgumentException("Usage: add-account Account_Name <balance>");
                    }

                    string accountName = input[1];

                    if (accountName.Contains(" "))
                    {
                        throw new ArgumentException("Account name cannot contain spaces. Use underscores instead.");
                    }

                    if (!decimal.TryParse(input[2], out decimal balance))
                    {
                        throw new FormatException("Invalid balance format. Use a valid decimal number.");
                    }

                    var account = accSvc.Create(accountName, balance);
                    Console.WriteLine($"Created Account ID: {account.Id}");
                    break;
                }
                
            case "edit-account":
                {
                    if (input.Length != 4)
                    {
                        throw new ArgumentException("Usage: edit-account Account_Name New_Account_Name <balance>");
                    }

                    string oldAccountName = input[1];
                    string newAccountName = input[2];

                    if (oldAccountName.Contains(" ") || newAccountName.Contains(" "))
                    {
                        throw new ArgumentException("Account name cannot contain spaces. Use underscores instead.");
                    }

                    if (!decimal.TryParse(input[3], out decimal balance))
                    {
                        throw new FormatException("Invalid balance format. Use a valid decimal number.");
                    }

                    var account = accSvc.GetAll().FirstOrDefault(a => a.Name == oldAccountName)
                        ?? throw new InvalidOperationException($"Account '{oldAccountName}' not found.");

                    accSvc.Update(account.Id, newAccountName, balance);
                    Console.WriteLine($"Updated Account: {oldAccountName} -> {newAccountName}");
                    break;
                }
                
            case "delete-account":
                {
                    if (input.Length != 2)
                    {
                        throw new ArgumentException("Usage: delete-account Account_Name");
                    }

                    string accountName = input[1];

                    if (accountName.Contains(" "))
                    {
                        throw new ArgumentException("Account name cannot contain spaces. Use underscores instead.");
                    }

                    var account = accSvc.GetAll().FirstOrDefault(a => a.Name == accountName)
                        ?? throw new InvalidOperationException($"Account '{accountName}' not found.");

                    accSvc.Delete(account.Id);
                    Console.WriteLine($"Deleted Account: {accountName}");
                    break;
                }

            case "add-category":
                {
                    if (input.Length != 3)
                    {
                        throw new ArgumentException("Usage: add-category Category_Name <Income/Expense>");
                    }

                    string categoryName = input[1];

                    if (categoryName.Contains(" "))
                    {
                        throw new ArgumentException("Category name cannot contain spaces. Use underscores instead.");
                    }

                    if (!Enum.TryParse<CategoryType>(input[2], true, out var categoryType))
                    {
                        throw new ArgumentException("Invalid category type. Use 'Income' or 'Expense'.");
                    }

                    var category = catSvc.Create(categoryName, categoryType);
                    Console.WriteLine($"Created Category: {category.Name}");
                    break;
                }
                
            case "edit-category":
                {
                    if (input.Length != 4)
                    {
                        throw new ArgumentException("Usage: edit-category Category_Name New_Category_Name <Income/Expense>");
                    }

                    string oldCategoryName = input[1];
                    string newCategoryName = input[2];

                    if (oldCategoryName.Contains(" ") || newCategoryName.Contains(" "))
                    {
                        throw new ArgumentException("Category name cannot contain spaces. Use underscores instead.");
                    }

                    if (!Enum.TryParse<CategoryType>(input[3], true, out var categoryType))
                    {
                        throw new ArgumentException("Invalid category type. Use 'Income' or 'Expense'.");
                    }

                    var category = catSvc.GetAll().FirstOrDefault(c => c.Name == oldCategoryName)
                        ?? throw new InvalidOperationException($"Category '{oldCategoryName}' not found.");

                    catSvc.Update(category.Id, newCategoryName, categoryType);
                    Console.WriteLine($"Updated Category: {oldCategoryName} -> {newCategoryName}");
                    break;
                }
                
            case "delete-category":
                {
                    if (input.Length != 2)
                    {
                        throw new ArgumentException("Usage: delete-category Category_Name");
                    }

                    string categoryName = input[1];

                    if (categoryName.Contains(" "))
                    {
                        throw new ArgumentException("Category name cannot contain spaces. Use underscores instead.");
                    }

                    var category = catSvc.GetAll().FirstOrDefault(c => c.Name == categoryName)
                        ?? throw new InvalidOperationException($"Category '{categoryName}' not found.");

                    catSvc.Delete(category.Id);
                    Console.WriteLine($"Deleted Category: {categoryName}");
                    break;
                }

            case "add-operation":
                {
                    if (input.Length < 5)
                    {
                        throw new ArgumentException("Usage: add-operation <Income/Expense> Account_Name <amount> Category_Name [Description]");
                    }

                    if (!Enum.TryParse<OperationType>(input[1], true, out var operationType))
                    {
                        throw new ArgumentException("Invalid operation type. Use 'Income' or 'Expense'.");
                    }

                    string operationAccountName = input[2];

                    if (operationAccountName.Contains(" "))
                    {
                        throw new ArgumentException("Account name cannot contain spaces. Use underscores instead.");
                    }

                    var selectedAccount = accSvc.GetAll().FirstOrDefault(a => a.Name == operationAccountName)
                        ?? throw new InvalidOperationException($"Account '{operationAccountName}' not found.");

                    if (!decimal.TryParse(input[3], out decimal amount) || amount <= 0)
                    {
                        throw new ArgumentException($"Invalid amount '{input[3]}'. Must be a positive number.");
                    }

                    string operationCategoryName = input[4];

                    if (operationCategoryName.Contains(" "))
                    {
                        throw new ArgumentException("Category name cannot contain spaces. Use underscores instead.");
                    }

                    var selectedCategory = catSvc.GetAll().FirstOrDefault(c => c.Name == operationCategoryName)
                        ?? throw new InvalidOperationException($"Category '{operationCategoryName}' not found.");

                    string description = input.Length > 5 ? string.Join(" ", input.Skip(5)) : "";

                    var operation = opSvc.Create(operationType, selectedAccount.Id, amount, DateTime.Now, selectedCategory.Id, description);
                    Console.WriteLine($"Created Operation ID: {operation.Id}");
                    break;
                }
                
            case "edit-operation":
                {
                    if (input.Length < 7)
                    {
                        throw new ArgumentException("Usage: edit-operation <Income/Expense> Account_Name <old-amount> Category_Name <new-type> <new-amount> [New_Description]");
                    }

                    if (!Enum.TryParse<OperationType>(input[1], true, out var oldOperationType))
                    {
                        throw new ArgumentException("Invalid operation type. Use 'Income' or 'Expense'.");
                    }

                    string accountName = input[2];

                    if (accountName.Contains(" "))
                    {
                        throw new ArgumentException("Account name cannot contain spaces. Use underscores instead.");
                    }

                    var account = accSvc.GetAll().FirstOrDefault(a => a.Name == accountName)
                        ?? throw new InvalidOperationException($"Account '{accountName}' not found.");

                    if (!decimal.TryParse(input[3], out decimal oldAmount) || oldAmount <= 0)
                    {
                        throw new ArgumentException($"Invalid old amount '{input[3]}'. Must be a positive number.");
                    }

                    string categoryName = input[4];

                    if (categoryName.Contains(" "))
                    {
                        throw new ArgumentException("Category name cannot contain spaces. Use underscores instead.");
                    }

                    var category = catSvc.GetAll().FirstOrDefault(c => c.Name == categoryName)
                        ?? throw new InvalidOperationException($"Category '{categoryName}' not found.");

                    
                    var operation = opSvc.GetAll().FirstOrDefault(o => 
                        o.Type == oldOperationType && 
                        o.BankAccountId == account.Id && 
                        Math.Abs(o.Amount - oldAmount) < 0.001m && 
                        o.CategoryId == category.Id);

                    if (operation == null)
                        throw new InvalidOperationException($"Operation with type {oldOperationType}, account {accountName}, amount {oldAmount}, and category {categoryName} not found.");

                    
                    if (!Enum.TryParse<OperationType>(input[5], true, out var newOperationType))
                    {
                        throw new ArgumentException("Invalid new operation type. Use 'Income' or 'Expense'.");
                    }

                    if (!decimal.TryParse(input[6], out decimal newAmount) || newAmount <= 0)
                    {
                        throw new ArgumentException($"Invalid new amount '{input[6]}'. Must be a positive number.");
                    }

                    string newDescription = input.Length > 7 ? string.Join(" ", input.Skip(7)) : "";

                    opSvc.Update(operation.Id, newOperationType, account.Id, newAmount, DateTime.Now, category.Id, newDescription);
                    Console.WriteLine($"Updated Operation: {oldOperationType} {accountName} {oldAmount} {categoryName} -> {newOperationType} {accountName} {newAmount} {categoryName}");
                    break;
                }
                
            case "delete-operation":
                {
                    if (input.Length < 5)
                    {
                        throw new ArgumentException("Usage: delete-operation <Income/Expense> Account_Name <amount> Category_Name");
                    }

                    if (!Enum.TryParse<OperationType>(input[1], true, out var operationType))
                    {
                        throw new ArgumentException("Invalid operation type. Use 'Income' or 'Expense'.");
                    }

                    string accountName = input[2];

                    if (accountName.Contains(" "))
                    {
                        throw new ArgumentException("Account name cannot contain spaces. Use underscores instead.");
                    }

                    var account = accSvc.GetAll().FirstOrDefault(a => a.Name == accountName)
                        ?? throw new InvalidOperationException($"Account '{accountName}' not found.");

                    if (!decimal.TryParse(input[3], out decimal amount) || amount <= 0)
                    {
                        throw new ArgumentException($"Invalid amount '{input[3]}'. Must be a positive number.");
                    }

                    string categoryName = input[4];

                    if (categoryName.Contains(" "))
                    {
                        throw new ArgumentException("Category name cannot contain spaces. Use underscores instead.");
                    }

                    var category = catSvc.GetAll().FirstOrDefault(c => c.Name == categoryName)
                        ?? throw new InvalidOperationException($"Category '{categoryName}' not found.");

                    
                    var operation = opSvc.GetAll().FirstOrDefault(o => 
                        o.Type == operationType && 
                        o.BankAccountId == account.Id && 
                        Math.Abs(o.Amount - amount) < 0.001m && 
                        o.CategoryId == category.Id);

                    if (operation == null)
                        throw new InvalidOperationException($"Operation with type {operationType}, account {accountName}, amount {amount}, and category {categoryName} not found.");

                    opSvc.Delete(operation.Id);
                    Console.WriteLine($"Deleted Operation: {operationType} {accountName} {amount} {categoryName}");
                    break;
                }

            case "report-balance":
                {
                    if (input.Length != 4)
                    {
                        throw new ArgumentException("Usage: report-balance Account_Name <start-date> <end-date>");
                    }

                    string balanceAccountName = input[1];

                    if (balanceAccountName.Contains(" "))
                    {
                        throw new ArgumentException("Account name cannot contain spaces. Use underscores instead.");
                    }

                    var balanceAccount = accSvc.GetAll().FirstOrDefault(a => a.Name == balanceAccountName)
                        ?? throw new InvalidOperationException($"Account '{balanceAccountName}' not found.");

                    var startDate = DateTime.ParseExact(input[2], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    var endDate = DateTime.ParseExact(input[3], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    ICommand reportCmd = new ReportBalanceCommand(opSvc, balanceAccount.Id, startDate, endDate);
                    
                    if (timeNextCommand)
                    {
                        reportCmd = new TimingDecorator(reportCmd);
                        timeNextCommand = false;
                    }
                    
                    await reportCmd.ExecuteAsync();
                    break;
                }

            case "report-category":
                {
                    if (input.Length != 2)
                    {
                        throw new ArgumentException("Usage: report-category Account_Name");
                    }

                    string categoryAccountName = input[1];

                    if (categoryAccountName.Contains(" "))
                    {
                        throw new ArgumentException("Account name cannot contain spaces. Use underscores instead.");
                    }

                    var categoryAccount = accSvc.GetAll().FirstOrDefault(a => a.Name == categoryAccountName)
                        ?? throw new InvalidOperationException($"Account '{categoryAccountName}' not found.");

                    ICommand categoryReportCmd = new ReportCategoryCommand(opSvc, catSvc, categoryAccount.Id);
                    
                    if (timeNextCommand)
                    {
                        categoryReportCmd = new TimingDecorator(categoryReportCmd);
                        timeNextCommand = false;
                    }
                    
                    await categoryReportCmd.ExecuteAsync();
                    break;
                }

            case "import":
                {
                    if (input.Length != 3)
                    {
                        throw new ArgumentException("Usage: import <json/csv/yaml> <filename>");
                    }

                    string format = input[1].ToLower();
                    string filePath = input[2];

                    ImporterBase? importer = format switch
                    {
                        "json" => new JsonImporter(accSvc, catSvc, opSvc),
                        "csv" => new CsvImporter(accSvc, catSvc, opSvc),
                        "yaml" => new YamlImporter(accSvc, catSvc, opSvc),
                        _ => throw new ArgumentException("Unsupported format")
                    };

                    ICommand importCmd = new ImportDataCommand(importer, filePath);
                    
                    if (timeNextCommand)
                    {
                        importCmd = new TimingDecorator(importCmd);
                        timeNextCommand = false;
                    }
                    
                    await importCmd.ExecuteAsync();
                    break;
                }

            case "export":
                {
                    if (input.Length != 3)
                    {
                        throw new ArgumentException("Usage: export <json/csv/yaml> <filename>");
                    }

                    string format = input[1].ToLower();
                    string filePath = input[2];

                    IExportVisitor? exporter = format switch
                    {
                        "json" => new JsonExportVisitor(),
                        "csv" => new CsvExportVisitor(),
                        "yaml" => new YamlExportVisitor(),
                        _ => throw new ArgumentException("Unsupported format")
                    };

                    ICommand exportCmd = new ExportDataCommand(exporter, filePath,
                        accSvc.GetRepository(), catSvc.GetRepository(), opSvc.GetRepository());

                    if (timeNextCommand)
                    {
                        exportCmd = new TimingDecorator(exportCmd);
                        timeNextCommand = false;
                    }

                    await exportCmd.ExecuteAsync();
                    break;
                }

            default:
                throw new InvalidOperationException($"Unknown command: {input[0]}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
