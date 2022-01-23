using Listjj.Abstract;
using Listjj.Data;
using Listjj.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Listjj.Repository
{
    public class ListItemRepository : GenericRepository<ListItem>, IListItemRepository
    {
        readonly AppDbContext _context;
        public ListItemRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ListItem>> GetAllByUserId(Guid id)
        {
            return await _context.ListItems.Include(i => i.Files).Include(i => i.Category).Where(x => x.UserId == id).ToListAsync();
        }

        public async Task<List<ListItem>> GetAllByCategoryId(Guid id)
        {
            return await _context.ListItems.Include(i => i.Files).Where(x => x.CategoryId == id).ToListAsync();
        }

        public async Task<List<ListItem>> ExecuteQuery(Expression<Func<ListItem, bool>> filter)
        {
            return await _context.ListItems.Include(i => i.Files).Where(filter).ToListAsync();
        }

    }
}
