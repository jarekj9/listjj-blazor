using System.Threading.Tasks;
using System.Collections.Generic;
using Listjj.Models;
using System;

namespace Listjj.Abstract
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<List<Category>> GetAllByUserId(Guid id);
        Task<Category> GetByName(string name);
    }
}