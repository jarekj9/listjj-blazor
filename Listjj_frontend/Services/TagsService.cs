﻿using Listjj_frontend.Services.Abstract;
using static MudBlazor.CategoryTypes;

namespace Listjj_frontend.Services
{
    public class TagsService : ITagsService
    {
        private readonly IApiClient apiClient;
        public TagsService(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<List<string>> GetByUserId(Guid userId)
        {
            var response = await apiClient.Get<List<string>>($"https://localhost:5001/api/tags/get_by_userid?id={userId}");
            var tags = response.HttpResponse.IsSuccessStatusCode ? response.Result : new List<string>();
            return tags;
        }
        public async Task<bool> UpdateByUserId(Guid userId, List<string> tags)
        {
            var response = await apiClient.Post<List<string>, bool>($"https://localhost:5001/api/tags/{userId}/update", tags);
            return response.HttpResponse.IsSuccessStatusCode;
        }
    }
}