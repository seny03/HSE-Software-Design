using System;
using System.Text.Json.Serialization;

namespace FinancialAccounting.Domain
{
    public class BankAccount
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }

        [JsonConstructor]
        public BankAccount() { }
        public BankAccount(string name, decimal balance)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException();
            if (balance < 0) throw new ArgumentException();

            Id = Guid.NewGuid();
            Name = name;
            Balance = balance;
        }

        public void Apply(OperationType type, decimal amount)
        {
            Balance += type == OperationType.Income ? amount : -amount;
        }
    }
}