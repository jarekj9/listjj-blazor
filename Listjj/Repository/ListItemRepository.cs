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
            return await _context.ListItems.Include(i => i.Files).Include(i => i.Category).Where(x => x.UserId == id).OrderBy(l => l.SequenceNumber).ToListAsync();
        }

        public async Task<List<ListItem>> GetAllByCategoryId(Guid id)
        {
            return await _context.ListItems.Include(i => i.Files).Include(i => i.Category).Where(x => x.CategoryId == id).ToListAsync();
        }

        public async Task<ListItem> GetByIdWithFiles(Guid id)
        {
            return await _context.ListItems.Include(i => i.Files).Include(i => i.Category).Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> Move(Guid id, string direction)
        {
            var movedItem = await _context.ListItems.FirstOrDefaultAsync(i => i.Id == id);
            var movedItemSequence = movedItem?.SequenceNumber ?? 0;

            if (direction == "up")
            { 
                var previousItem = await _context.ListItems.Where(i => i.CategoryId == movedItem.CategoryId && i.SequenceNumber < movedItemSequence).OrderBy(i => i.SequenceNumber).LastOrDefaultAsync();
                var previousItemSequence = previousItem?.SequenceNumber ?? -1;
                if (previousItemSequence == -1)
                {
                    return false;
                }
                movedItem.SequenceNumber = previousItemSequence;
                previousItem.SequenceNumber = movedItemSequence;
            }

            if (direction == "down")
            {
                var nextItem = await _context.ListItems.Where(i => i.CategoryId == movedItem.CategoryId && i.SequenceNumber > movedItemSequence).OrderBy(i => i.SequenceNumber).FirstOrDefaultAsync();
                var nextItemSequence = nextItem?.SequenceNumber ?? -1;
                if (nextItemSequence == -1)
                {
                    return false;
                }
                movedItem.SequenceNumber = nextItemSequence;
                nextItem.SequenceNumber = movedItemSequence;
            }

            return true;
        }

        public async Task<List<ListItem>> ExecuteQuery(Expression<Func<ListItem, bool>> filter)
        {
            return await _context.ListItems.Include(i => i.Files).Include(i => i.Category).Where(filter).OrderBy(i => i.SequenceNumber).ToListAsync();
        }

    }
}
