using System;
using System.Collections.Generic;
using System.Linq;
using FinancialAccounting.Domain;
using FinancialAccounting.Persistence;

namespace FinancialAccounting.Services
{
    public class OperationService
    {
        private readonly IRepository<Operation> _repo;
        private readonly IRepository<BankAccount> _accRepo;

        public OperationService(IRepository<Operation> repo, IRepository<BankAccount> accRepo)
        {
            _repo = repo;
            _accRepo = accRepo;
        }

        public IRepository<Operation> GetRepository() => _repo;

        public Operation Create(OperationType type, Guid accountId, decimal amount, DateTime date, Guid categoryId, string description = "", bool skipBalanceUpdate = false, Guid? id = null)
        {
            var operation = new Operation(id ?? Guid.NewGuid(), type, accountId, amount, date, categoryId, description);

            _repo.Add(operation);

            var account = _accRepo.Get(accountId);
            if (account == null)
            {
                throw new InvalidOperationException($"Cannot find account with ID {accountId} while creating operation.");
            }

            if (!skipBalanceUpdate)
            {
                account.Apply(type, amount);
            }

            return operation;
        }

        public void Update(Guid id, OperationType type, Guid accountId, decimal amount, DateTime date, Guid categoryId, string description = "")
        {
            var operation = _repo.Get(id);
            if (operation == null)
                throw new InvalidOperationException($"Operation with ID {id} not found.");
            
            var account = _accRepo.Get(accountId);
            if (account == null)
                throw new InvalidOperationException($"Account with ID {accountId} not found.");
            
            
            if (operation.BankAccountId == accountId)
            {
                account.Apply(operation.Type == OperationType.Income ? OperationType.Expense : OperationType.Income, operation.Amount);
            }
            else
            {
                
                var oldAccount = _accRepo.Get(operation.BankAccountId);
                if (oldAccount != null)
                {
                    oldAccount.Apply(operation.Type == OperationType.Income ? OperationType.Expense : OperationType.Income, operation.Amount);
                }
            }
            
            
            operation.Type = type;
            operation.BankAccountId = accountId;
            operation.Amount = amount;
            operation.Date = date;
            operation.CategoryId = categoryId;
            operation.Description = description;
            
            
            account.Apply(type, amount);
            
            _repo.Update(operation);
        }
        
        public void Delete(Guid id)
        {
            var operation = _repo.Get(id);
            if (operation == null)
                throw new InvalidOperationException($"Operation with ID {id} not found.");
            
            
            var account = _accRepo.Get(operation.BankAccountId);
            if (account != null)
            {
                account.Apply(operation.Type == OperationType.Income ? OperationType.Expense : OperationType.Income, operation.Amount);
            }
            
            _repo.Delete(id);
        }

        public decimal BalanceDifference(Guid accountId, DateTime from, DateTime to)
        {
            return _repo.GetAll()
                .Where(o => o.BankAccountId == accountId && o.Date >= from && o.Date <= to)
                .Sum(o => o.Type == OperationType.Income ? o.Amount : -o.Amount);
        }

        public Dictionary<string, decimal> GroupByCategory(Guid accountId, CategoryService categoryService)
        {
            return _repo.GetAll()
                .Where(o => o.BankAccountId == accountId)
                .GroupBy(o => o.CategoryId)
                .ToDictionary(
                    g => categoryService.GetAll().FirstOrDefault(c => c.Id == g.Key)?.Name ?? "Unknown",
                    g => g.Sum(o => o.Type == OperationType.Income ? o.Amount : -o.Amount)
                );
        }

        public IEnumerable<Operation> GetAll() => _repo.GetAll();
    }
}
