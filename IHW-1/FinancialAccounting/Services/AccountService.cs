using System;
using System.Linq;
using FinancialAccounting.Domain;
using FinancialAccounting.Persistence;

namespace FinancialAccounting.Services
{
    public class AccountService
    {
        private readonly IRepository<BankAccount> _repo;

        public AccountService(IRepository<BankAccount> repository)
        {
            _repo = repository;
        }

        public IRepository<BankAccount> GetRepository() => _repo;

        public BankAccount Create(string name, decimal balance, Guid? id = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Account name cannot be empty.");

            var account = new BankAccount(name, balance)
            {
                Id = id ?? Guid.NewGuid()  
            };

            _repo.Add(account);
            return account;
        }

        public void Update(Guid id, string name, decimal balance)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Account name cannot be empty.");
            
            var account = _repo.Get(id);
            if (account == null)
                throw new InvalidOperationException($"Account with ID {id} not found.");
            
            account.Name = name;
            account.Balance = balance;
            
            _repo.Update(account);
        }
        
        public void Delete(Guid id)
        {
            var account = _repo.Get(id);
            if (account == null)
                throw new InvalidOperationException($"Account with ID {id} not found.");
            
            _repo.Delete(id);
        }

        public IEnumerable<BankAccount> GetAll() => _repo.GetAll();
    }
}
