using System;
using System.Collections.Generic;

namespace FinancialAccounting.Persistence
{
    public interface IRepository<T>
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(Guid id);
        T Get(Guid id);
        IEnumerable<T> GetAll();
    }
}
