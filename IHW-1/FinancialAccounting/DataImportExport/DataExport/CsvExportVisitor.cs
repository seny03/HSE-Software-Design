using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using FinancialAccounting.Domain;
using FinancialAccounting.Persistence;

namespace FinancialAccounting.DataImportExport.DataExport
{
    public class CsvExportVisitor : IExportVisitor
    {
        private readonly List<BankAccount> _accounts = new();
        private readonly List<Category> _categories = new();
        private readonly List<Operation> _operations = new();

        public void Visit(BankAccount account) => _accounts.Add(account);
        public void Visit(Category category) => _categories.Add(category);
        public void Visit(Operation operation) => _operations.Add(operation);

        public void ExportToFile(string filePath, IRepository<BankAccount> accRepo, IRepository<Category> catRepo, IRepository<Operation> opRepo)
        {
            
            foreach (var account in accRepo.GetAll()) Visit(account);
            foreach (var category in catRepo.GetAll()) Visit(category);
            foreach (var operation in opRepo.GetAll()) Visit(operation);

            
            string baseFilePath = Path.Combine(
                Path.GetDirectoryName(filePath) ?? "",
                Path.GetFileNameWithoutExtension(filePath)
            );
            
            
            string accountsFilePath = $"{baseFilePath}_accounts.csv";
            string categoriesFilePath = $"{baseFilePath}_categories.csv";
            string operationsFilePath = $"{baseFilePath}_operations.csv";

            
            using (var writer = new StreamWriter(accountsFilePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.WriteRecords(_accounts);
            }

            
            using (var writer = new StreamWriter(categoriesFilePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.WriteRecords(_categories);
            }

            
            using (var writer = new StreamWriter(operationsFilePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.WriteRecords(_operations);
            }

            Console.WriteLine($"Exported {_accounts.Count} accounts to {accountsFilePath}");
            Console.WriteLine($"Exported {_categories.Count} categories to {categoriesFilePath}");
            Console.WriteLine($"Exported {_operations.Count} operations to {operationsFilePath}");
        }
    }
}
