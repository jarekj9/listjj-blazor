using System.Threading.Tasks;
using System.Collections.Generic;
using Listjj.Models;
using System;
using System.Linq.Expressions;

namespace Listjj.Abstract
{
    public interface IListItemRepository : IGenericRepository<ListItem>
    {
        Task<List<ListItem>> GetAllByUserId(string id);
        Task<List<ListItem>> GetAllByCategoryId(Guid id);
        Task<List<ListItem>> ExecuteQuery(Expression<Func<ListItem, bool>> filter);
    }
}