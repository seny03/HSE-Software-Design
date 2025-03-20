using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FinancialAccounting.Domain;
using FinancialAccounting.Persistence;
using YamlDotNet.Serialization;

namespace FinancialAccounting.DataImportExport.DataExport
{
    public class YamlExportVisitor : IExportVisitor
    {
        private readonly List<BankAccount> _accounts = new();
        private readonly List<Category> _categories = new();
        private readonly List<Operation> _operations = new();

        public void Visit(BankAccount account) => _accounts.Add(account);
        public void Visit(Category category) => _categories.Add(category);
        public void Visit(Operation operation) => _operations.Add(operation);

        public void ExportToFile(string filePath, IRepository<BankAccount> accRepo, IRepository<Category> catRepo, IRepository<Operation> opRepo)
        {
            try
            {
                
                _accounts.Clear();
                _categories.Clear();
                _operations.Clear();

                
                foreach (var account in accRepo.GetAll()) Visit(account);
                foreach (var category in catRepo.GetAll()) Visit(category);
                foreach (var operation in opRepo.GetAll()) Visit(operation);

                
                var yamlData = new YamlDataStructure
                {
                    Accounts = _accounts.Select(a => new YamlAccount
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Balance = a.Balance
                    }).ToList(),

                    Categories = _categories.Select(c => new YamlCategory
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Type = (int)c.Type
                    }).ToList(),

                    Operations = _operations.Select(o => new YamlOperation
                    {
                        Id = o.Id,
                        Type = (int)o.Type,
                        BankAccountId = o.BankAccountId,
                        Amount = o.Amount,
                        Date = o.Date,
                        CategoryId = o.CategoryId,
                        Description = o.Description
                    }).ToList()
                };

                
                var serializer = new SerializerBuilder()
                    .WithIndentedSequences()
                    .ConfigureDefaultValuesHandling(YamlDotNet.Serialization.DefaultValuesHandling.Preserve)
                    .Build();

                
                var yaml = serializer.Serialize(yamlData);

                
                File.WriteAllText(filePath, yaml);
                
                Console.WriteLine($"Exported {_accounts.Count} accounts, {_categories.Count} categories, {_operations.Count} operations to YAML file: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during YAML export: {ex.Message}");
                throw;
            }
        }

        
        private class YamlDataStructure
        {
            public List<YamlAccount> Accounts { get; set; } = new();
            public List<YamlCategory> Categories { get; set; } = new();
            public List<YamlOperation> Operations { get; set; } = new();
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
