using System;
using Xunit;
using Moq;
using FinancialAccounting.Domain;
using FinancialAccounting.Persistence;
using FinancialAccounting.Services;
using System.Collections.Generic;
using System.Linq;

namespace FinancialAccounting.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IRepository<Category>> _mockRepository;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _mockRepository = new Mock<IRepository<Category>>();
            _service = new CategoryService(_mockRepository.Object);
        }

        [Fact]
        public void GetRepository_ReturnsRepository()
        {
            
            var repository = _service.GetRepository();

            
            Assert.Same(_mockRepository.Object, repository);
        }

        [Fact]
        public void Create_WithValidParameters_CreatesAndAddsCategory()
        {
            
            string name = "TestCategory";
            CategoryType type = CategoryType.Income;
            Category addedCategory = null;

            _mockRepository.Setup(r => r.Add(It.IsAny<Category>()))
                .Callback<Category>(category => addedCategory = category);

            
            var result = _service.Create(name, type);

            
            Assert.NotNull(result);
            Assert.Equal(name, result.Name);
            Assert.Equal(type, result.Type);
            Assert.NotEqual(Guid.Empty, result.Id);
            
            _mockRepository.Verify(r => r.Add(It.IsAny<Category>()), Times.Once);
            Assert.Same(result, addedCategory);
        }

        [Fact]
        public void Create_WithSpecifiedId_CreatesCategoryWithThatId()
        {
            
            string name = "TestCategory";
            CategoryType type = CategoryType.Expense;
            Guid id = Guid.NewGuid();

            
            var result = _service.Create(name, type, id);

            
            Assert.Equal(id, result.Id);
            _mockRepository.Verify(r => r.Add(It.Is<Category>(c => c.Id == id)), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidName_ThrowsArgumentException(string invalidName)
        {
            
            CategoryType type = CategoryType.Income;

            
            Assert.Throws<ArgumentException>(() => _service.Create(invalidName, type));
            _mockRepository.Verify(r => r.Add(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public void Update_WithValidParameters_UpdatesCategory()
        {
            
            var id = Guid.NewGuid();
            var existingCategory = new Category(id, "OldName", CategoryType.Income);
            string newName = "NewName";
            CategoryType newType = CategoryType.Expense;

            _mockRepository.Setup(r => r.Get(id)).Returns(existingCategory);

            
            _service.Update(id, newName, newType);

            
            Assert.Equal(newName, existingCategory.Name);
            Assert.Equal(newType, existingCategory.Type);
            _mockRepository.Verify(r => r.Update(existingCategory), Times.Once);
        }

        [Fact]
        public void Update_WithNonExistingId_ThrowsInvalidOperationException()
        {
            
            var id = Guid.NewGuid();
            _mockRepository.Setup(r => r.Get(id)).Returns((Category)null);

            
            Assert.Throws<InvalidOperationException>(() => 
                _service.Update(id, "NewName", CategoryType.Income));
            _mockRepository.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WithInvalidName_ThrowsArgumentException(string invalidName)
        {
            
            var id = Guid.NewGuid();
            CategoryType type = CategoryType.Income;

            
            Assert.Throws<ArgumentException>(() => _service.Update(id, invalidName, type));
            _mockRepository.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public void Delete_ExistingCategory_DeletesCategory()
        {
            
            var id = Guid.NewGuid();
            var existingCategory = new Category(id, "TestCategory", CategoryType.Income);
            _mockRepository.Setup(r => r.Get(id)).Returns(existingCategory);

            
            _service.Delete(id);

            
            _mockRepository.Verify(r => r.Delete(id), Times.Once);
        }

        [Fact]
        public void Delete_NonExistingCategory_ThrowsInvalidOperationException()
        {
            
            var id = Guid.NewGuid();
            _mockRepository.Setup(r => r.Get(id)).Returns((Category)null);

            
            Assert.Throws<InvalidOperationException>(() => _service.Delete(id));
            _mockRepository.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public void GetAll_ReturnsAllCategories()
        {
            
            var categories = new List<Category>
            {
                new Category("Category1", CategoryType.Income) { Id = Guid.NewGuid() },
                new Category("Category2", CategoryType.Expense) { Id = Guid.NewGuid() },
                new Category("Category3", CategoryType.Income) { Id = Guid.NewGuid() }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(categories);

            
            var result = _service.GetAll().ToList();

            
            Assert.Equal(categories.Count, result.Count);
            foreach (var category in categories)
            {
                Assert.Contains(result, c => c.Id == category.Id);
            }
        }
    }
}
