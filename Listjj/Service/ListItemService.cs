using System;
using System.Data;
using MySql.Data.MySqlClient;  // mysql
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Listjj.Abstract;
using Listjj.Models;
using Listjj.Data;

 
namespace Listjj.Service
{
    public class ListItemService : IListItemService
    {
        private readonly AppDbContext  _appDbContext;

        public ListItemService (AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<ListItem> FindById(Guid id)
        {
            return (await _appDbContext.ListItems.FindAsync(id));
        }

        public async Task<bool> AddListItem(ListItem item)
        {  
            item.Created = DateTime.Now;
            item.Modified = DateTime.Now;
            await _appDbContext.ListItems.AddAsync(item);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DelListItem(ListItem item)
        {
            _appDbContext.ListItems.Remove(item);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateListItem(ListItem item)
        {
            item.Modified = DateTime.Now;
            _appDbContext.ListItems.Update(item);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<ListItem>> GetAllItems()
        {
            return await _appDbContext.ListItems.Include(i => i.Files).ToListAsync();
        }
        public async Task<List<ListItem>> GetItemsByUserId(string id)
        {
            return await _appDbContext.ListItems.Include(i => i.Files).Include(i => i.Category).Where(x => x.UserId == id).ToListAsync();
        }
        public async Task<List<ListItem>> GetItemsByCategoryId(Guid id)
        {
            return await _appDbContext.ListItems.Include(i => i.Files).Where(x => x.CategoryId == id).ToListAsync();
        }
        public async Task<List<ListItem>> ExecuteQuery(Expression<Func<ListItem, bool>> filter)
        {
            IQueryable<ListItem> query = _appDbContext.ListItems.Include(i => i.Files).Where(filter);
            return await query.ToListAsync();
        }
    }
}