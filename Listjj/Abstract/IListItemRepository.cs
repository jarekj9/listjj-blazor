using System.Threading.Tasks;
using System.Collections.Generic;
using Listjj.Models;
using System;
using System.Linq.Expressions;

namespace Listjj.Abstract
{
    public interface IListItemRepository : IGenericRepository<ListItem>
    {
        Task<List<ListItem>> GetAllByUserId(Guid id);
        Task<List<ListItem>> GetAllByCategoryId(Guid id);
        Task<ListItem> GetByIdWithFiles(Guid id);
        Task<List<ListItem>> ExecuteQuery(Expression<Func<ListItem, bool>> filter);
        Task<bool> Move(Guid id, string direction);
    }
}