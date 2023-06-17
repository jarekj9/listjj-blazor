using Listjj.Infrastructure.ViewModels;

namespace Listjj_frontend.Services.Abstract
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
