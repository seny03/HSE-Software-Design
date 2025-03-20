using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancialAccounting.Persistence
{
    public class InMemoryRepository<T> : IRepository<T> where T : class
    {
        private readonly Dictionary<Guid, T> _store = new();
        private readonly Func<T, Guid> _idSelector;

        public InMemoryRepository(Func<T, Guid> idSelector) 
            => _idSelector = idSelector;

        public void Add(T entity) => _store[_idSelector(entity)] = entity;
        public void Update(T entity) => _store[_idSelector(entity)] = entity;
        public void Delete(Guid id) => _store.Remove(id);
        public T? Get(Guid id) => _store.TryGetValue(id, out var e) ? e : null;
        public IEnumerable<T> GetAll() => _store.Values.ToList();
    }
}
