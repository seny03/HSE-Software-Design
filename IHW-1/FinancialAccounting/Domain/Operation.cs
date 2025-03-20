using System;
using System.Text.Json.Serialization;

namespace FinancialAccounting.Domain
{
    public enum OperationType { Income, Expense }

    public class Operation
    {
        public Guid Id { get; set; }
        public OperationType Type { get; set; }
        public Guid BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Guid CategoryId { get; set; }
        public string Description { get; set; }

        [JsonConstructor]
        public Operation(Guid id, OperationType type, Guid bankAccountId, decimal amount, DateTime date, Guid categoryId, string description = "")
        {
            Id = id;
            Type = type;
            BankAccountId = bankAccountId;
            Amount = amount;
            Date = date;
            CategoryId = categoryId;
            Description = description;
        }

        public Operation(OperationType type, Guid accountId, decimal amount, DateTime date, Guid categoryId, string description = "")
        {
            Id = Guid.NewGuid();
            Type = type;
            BankAccountId = accountId;
            Amount = amount;
            Date = date;
            CategoryId = categoryId;
            Description = description;
        }
    }
}
