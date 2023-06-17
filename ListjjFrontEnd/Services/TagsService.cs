using ListjjFrontEnd.Services.Abstract;

namespace ListjjFrontEnd.Services
{
    public class TagsService : ITagsService
    {
        private readonly IApiClient apiClient;
        public TagsService(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<List<string>> GetByUserId()
        {
            var response = await apiClient.Get<List<string>>($"/api/tags/get_by_userid");
            var tags = response.HttpResponse.IsSuccessStatusCode ? response.Result : new List<string>();
            return tags;
        }
        public async Task<bool> UpdateByUserId(List<string> tags)
        {
            var response = await apiClient.Post<List<string>, bool>($"/api/tags/update", tags);
            return response.HttpResponse.IsSuccessStatusCode;
        }
    }
}
