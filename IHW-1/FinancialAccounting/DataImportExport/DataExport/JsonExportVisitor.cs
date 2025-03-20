using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FinancialAccounting.Domain;
using FinancialAccounting.Persistence;

namespace FinancialAccounting.DataImportExport.DataExport
{
    public class JsonExportVisitor : IExportVisitor
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

            var exportData = new
            {
                Accounts = _accounts,
                Categories = _categories,
                Operations = _operations
            };

            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}
