using FinancialAccounting.Domain;
using FinancialAccounting.Persistence;

namespace FinancialAccounting.Services
{
    public class CategoryService
    {
        private readonly IRepository<Category> _repo;

        public CategoryService(IRepository<Category> repo) => _repo = repo;

        public IRepository<Category> GetRepository() => _repo;

        public Category Create(string name, CategoryType type, Guid? id = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty.");

            var category = new Category(id ?? Guid.NewGuid(), name, type);
            _repo.Add(category);
            return category;
        }
        
        public void Update(Guid id, string name, CategoryType type)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty.");
            
            var category = _repo.Get(id);
            if (category == null)
                throw new InvalidOperationException($"Category with ID {id} not found.");
            
            category.Name = name;
            category.Type = type;
            
            _repo.Update(category);
        }
        
        public void Delete(Guid id)
        {
            var category = _repo.Get(id);
            if (category == null)
                throw new InvalidOperationException($"Category with ID {id} not found.");
            
            _repo.Delete(id);
        }
        
        public IEnumerable<Category> GetAll() => _repo.GetAll();
    }
}
