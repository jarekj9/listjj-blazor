using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Listjj.Models;
using System.Linq.Expressions;

namespace Listjj.Abstract
{
    public interface IListjjervice
    {
        Task<ListItem> FindById(Guid id);
        Task<bool> AddListItem(ListItem item);
        Task<bool> DelListItem(ListItem item);
        Task<bool> UpdateListItem(ListItem item);
        Task<List<ListItem>> GetAllItems();
        Task<List<ListItem>> GetItemsByUserId(string id);
        Task<List<ListItem>> GetItemsByCategoryId(Guid id);
        Task<List<ListItem>> ExecuteQuery(Expression<Func<ListItem, bool>> filter);
    }
}