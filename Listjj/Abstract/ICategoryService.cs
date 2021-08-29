using System.Threading.Tasks;
using System.Collections.Generic;
using Listjj.Models;
using System;

namespace Listjj.Abstract
{
    public interface ICategoryService
    {
        Task<bool> AddCategory(Category category);
        Task<bool> DelCategory(Category category);
        Task<bool> UpdateCategory(Category category);
        Task<List<Category>> GetAllCategories();
        Task<Category> FindById(Guid categoryId);
        Task<Category> FindByName(string name);
        Task<List<Category>> GetCategoriesByUserId(string id);
    }
}