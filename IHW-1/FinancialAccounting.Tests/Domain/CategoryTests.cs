using System;
using Xunit;
using FinancialAccounting.Domain;

namespace FinancialAccounting.Tests.Domain
{
    public class CategoryTests
    {
        [Fact]
        public void Constructor_WithNameAndType_CreatesCategory()
        {
            
            string name = "TestCategory";
            CategoryType type = CategoryType.Income;

            
            var category = new Category(name, type);

            
            Assert.Equal(name, category.Name);
            Assert.Equal(type, category.Type);
            Assert.NotEqual(Guid.Empty, category.Id);
        }

        [Fact]
        public void Constructor_WithIdNameAndType_CreatesCategoryWithSpecifiedId()
        {
            
            Guid id = Guid.NewGuid();
            string name = "TestCategory";
            CategoryType type = CategoryType.Expense;

            
            var category = new Category(id, name, type);

            
            Assert.Equal(id, category.Id);
            Assert.Equal(name, category.Name);
            Assert.Equal(type, category.Type);
        }

        [Theory]
        [InlineData(CategoryType.Income)]
        [InlineData(CategoryType.Expense)]
        public void Constructor_WithDifferentCategoryTypes_SetsTypeCorrectly(CategoryType type)
        {
            
            string name = "TestCategory";

            
            var category = new Category(name, type);

            
            Assert.Equal(type, category.Type);
        }
    }
}
