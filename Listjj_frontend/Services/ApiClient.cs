using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Listjj_frontend.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly string apiEndpoint;

        public ApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            apiEndpoint = configuration.GetValue<string>("ApiEndpoint");
        }

        public async Task<(TResponse Result, HttpResponseMessage HttpResponse)> Get<TResponse>(string urlPart)
        {
            var response = await httpClient.GetAsync($"{apiEndpoint}{urlPart}");
            if (response?.Content == null)
            {
                return (default, null);
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            return (JsonConvert.DeserializeObject<TResponse>(responseContent), response);
        }

        public async Task<(TResponse Result, HttpResponseMessage HttpResponse)> Post<TRequest, TResponse>(string urlPart, TRequest requestData)
        {
            var response = await httpClient.PostAsJsonAsync($"{apiEndpoint}{urlPart}", requestData);
            if (response?.Content == null)
            {
                return (default, null);
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            return (JsonConvert.DeserializeObject<TResponse>(responseContent), response);
        }
    }

}
