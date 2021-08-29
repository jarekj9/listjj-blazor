using System;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Listjj.Abstract;
using Listjj.Models;
using Listjj.Data;

 

namespace Listjj.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext  _appDbContext;

        public CategoryService (AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> AddCategory(Category category)
        {  
            await _appDbContext.Categories.AddAsync(category);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DelCategory(Category category)
        {
            _appDbContext.Categories.Remove(category);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateCategory(Category category)
        {
            _appDbContext.Categories.Update(category);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<Category>> GetAllCategories()
        {
            return await _appDbContext.Categories.ToListAsync();
        }
        public async Task<List<Category>> GetCategoriesByUserId(string id)
        {
            return await _appDbContext.Categories.Where(x => x.UserId == id).ToListAsync();
        }

        public async Task<Category> FindById(Guid categoryId)
        {
            return (await _appDbContext.Categories.FindAsync(categoryId));
        }

        public async Task<Category> FindByName(string name)
        {
            return await Task.FromResult(_appDbContext.Categories.Where(x => x.Name == name).FirstOrDefault());
        }
    }
}