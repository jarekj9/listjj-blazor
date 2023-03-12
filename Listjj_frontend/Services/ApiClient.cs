using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json;

namespace Listjj_frontend.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient httpClient;

        public ApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<(TResponse Result, HttpResponseMessage HttpResponse)> Get<TResponse>(string url)
        {
            var response = await httpClient.GetAsync(url);
            if (response?.Content == null)
            {
                return (default, null);
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            return (JsonConvert.DeserializeObject<TResponse>(responseContent), response);
        }

        public async Task<(TResponse Result, HttpResponseMessage HttpResponse)> Post<TRequest, TResponse>(string url, TRequest requestData)
        {
            var response = await httpClient.PostAsJsonAsync(url, requestData);
            if (response?.Content == null)
            {
                return (default, null);
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            return (JsonConvert.DeserializeObject<TResponse>(responseContent), response);
        }
    }

}
