using ListjjFrontEnd.Services.Abstract;

namespace ListjjFrontEnd.Services
{
    public class ExternalAccessApiService : IExternalAccessApiService
    {
        private readonly IApiClient apiClient;
        public ExternalAccessApiService(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<string> GetApiKey()
        {
            var response = await apiClient.Get<string>($"/api/externalapikey/get");
            var key = response.HttpResponse.IsSuccessStatusCode ? response.Result : string.Empty;
            return key;  
        }
        public async Task<string> GenerateApiKey()
        {
            var response = await apiClient.Get<string>($"/api/externalapikey/generate");
            var key = response.HttpResponse.IsSuccessStatusCode ? response.Result : string.Empty;
            return key;
        }
    }
}
