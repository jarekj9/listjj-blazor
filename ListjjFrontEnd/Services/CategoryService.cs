﻿using Listjj.Infrastructure.DTOs;
using Listjj.Infrastructure.ViewModels;
using ListjjFrontEnd.Services.Abstract;

namespace ListjjFrontEnd.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IApiClient apiClient;
        public CategoryService(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<List<CategoryViewModel>> GetAllByUserId()
        {
            var response = await apiClient.Get<List<CategoryViewModel>>($"/api/category/categories_by_userid");
            var categories = response.HttpResponse.IsSuccessStatusCode ? response.Result : new List<CategoryViewModel>();
            return categories;
        }

        public async Task<CategoryViewModel> GetById(Guid id)
        {
            var response = await apiClient.Get<CategoryViewModel>($"/api/category/category_by_id?id={id}");
            var category = response.HttpResponse.IsSuccessStatusCode ? response.Result : new CategoryViewModel();
            return category;
        }

        public async Task<bool> AddorUpdateCategory(CategoryViewModel category)
        {
            var response = await apiClient.Post<CategoryViewModel, bool>($"/api/category/addorupdate", category);
            return response.HttpResponse.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategory(Guid id)
        {
            var response = await apiClient.Post<Guid, bool>($"/api/category/delete", id);
            return response.HttpResponse.IsSuccessStatusCode;
        }

        public async Task<Guid> GetRecentCategoryByUserId()
        {
            var response = await apiClient.Get<Guid>($"/api/category/recent_categoryid_by_userid");
            var categoryId = response.HttpResponse.IsSuccessStatusCode ? response.Result : Guid.Empty;
            return categoryId;
        }

        public async Task<bool> UpdateRecentCategory(Guid recentCategoryId)
        {
            var updateCategoryRequest = new UpdateCategoryRequest() { RecentCategoryId = recentCategoryId };
            var response = await apiClient.Post<UpdateCategoryRequest, bool>(
                $"/api/category/update_recent_category", updateCategoryRequest
            );
            return response.HttpResponse.IsSuccessStatusCode;
        }
    }
}
