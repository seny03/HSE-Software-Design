using System;
using System.Text.Json.Serialization;

namespace FinancialAccounting.Domain
{
    public enum CategoryType { Income, Expense }

    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public CategoryType Type { get; set; }

        [JsonConstructor]
        public Category(Guid id, string name, CategoryType type)
        {
            Id = id;
            Name = name;
            Type = type;
        }

        public Category(string name, CategoryType type)
        {
            Id = Guid.NewGuid();
            Name = name;
            Type = type;
        }
    }
}
