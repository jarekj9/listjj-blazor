using Listjj.Infrastructure.ViewModels;
using Listjj_frontend.Services.Abstract;

namespace Listjj_frontend.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IApiClient apiClient;
        public CategoryService(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<List<CategoryViewModel>> GetAllByUserId(Guid userId)
        {
            var response = await apiClient.Get<List<CategoryViewModel>>($"https://localhost:5001/api/category/categories_by_userid?userId={userId}");
            var categories = response.HttpResponse.IsSuccessStatusCode ? response.Result : new List<CategoryViewModel>();
            return categories;
        }

        public async Task<CategoryViewModel> GetById(Guid id)
        {
            var response = await apiClient.Get<CategoryViewModel>($"https://localhost:5001/api/category/category_by_id?id={id}");
            var category = response.HttpResponse.IsSuccessStatusCode ? response.Result : new CategoryViewModel();
            return category;
        }

        public async Task<bool> AddorUpdateCategory(CategoryViewModel category)
        {
            var response = await apiClient.Post<CategoryViewModel, bool>($"https://localhost:5001/api/category/addorupdate", category);
            return response.HttpResponse.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategory(Guid id)
        {
            var response = await apiClient.Post<Guid, bool>($"https://localhost:5001/api/category/delete", id);
            return response.HttpResponse.IsSuccessStatusCode;
        }
    }
}
