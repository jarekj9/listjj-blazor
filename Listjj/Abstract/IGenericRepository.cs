using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Listjj.Abstract
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(Guid id);
        void Add(T obj);
        void Update(T obj);
        void Delete(Guid id);
        Task<bool> Save();
    }
}
