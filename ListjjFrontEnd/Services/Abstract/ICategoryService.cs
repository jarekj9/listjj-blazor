using Listjj.Infrastructure.ViewModels;

namespace ListjjFrontEnd.Services.Abstract
{
    public interface ICategoryService
    {
        Task<List<CategoryViewModel>> GetAllByUserId();
        Task<CategoryViewModel> GetById(Guid id);
        Task<bool> AddorUpdateCategory(CategoryViewModel category);
        Task<bool> DeleteCategory(Guid id);
        Task<Guid> GetRecentCategoryByUserId();
        Task<bool> UpdateRecentCategory(Guid recentCategoryId);
    }
}
