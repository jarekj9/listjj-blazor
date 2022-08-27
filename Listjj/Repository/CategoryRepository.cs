using Listjj.Abstract;
using Listjj.Data;
using Listjj.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Listjj.Repository
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetAllByUserId(Guid id)
        {
            return await _context.Categories.Where(x => x.UserId == id).ToListAsync();
        }

        public async Task<Category> GetByName(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.Name == name);
        }

    }
}
