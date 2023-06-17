using Blazored.LocalStorage;
using ListjjFrontEnd.Services.Abstract;
using ListjjFrontEnd.Services.Abstract.Authentication;
using ListjjFrontEnd.Services.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ListjjFrontEnd.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly string apiEndpoint;
        private readonly ILocalStorageService localStorage;

        public ApiClient(HttpClient httpClient, IConfiguration configuration, ILocalStorageService localStorage)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            this.localStorage = localStorage;
            apiEndpoint = configuration.GetValue<string>("ApiEndpoint");
        }

        public async Task<(TResponse Result, HttpResponseMessage HttpResponse)> Get<TResponse>(string urlPart)
        {
            await SetAuthToken();

            var response = await httpClient.GetAsync($"{apiEndpoint}{urlPart}");
            if (response?.Content == null)
            {
                return (default, null);
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            TResponse jsonResponseContent;
            try
            {
                jsonResponseContent = JsonConvert.DeserializeObject<TResponse>(responseContent);
            }
            catch (Exception e)
            {
                jsonResponseContent = default;
            }
            return (JsonConvert.DeserializeObject<TResponse>(responseContent), response);
        }

        public async Task<(TResponse Result, HttpResponseMessage HttpResponse)> Post<TRequest, TResponse>(string urlPart, TRequest requestData)
        {
            await SetAuthToken();

            var response = await httpClient.PostAsJsonAsync($"{apiEndpoint}{urlPart}", requestData);
            if (response?.Content == null)
            {
                return (default, null);
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            TResponse jsonResponseContent;
            try
            {
                jsonResponseContent = JsonConvert.DeserializeObject<TResponse>(responseContent);
            }
            catch(Exception e)
            {
                jsonResponseContent = default;
            }
            return (jsonResponseContent, response);
        }

        private async Task SetAuthToken()
        {
            var savedToken = await localStorage.GetItemAsync<string>("authToken");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
        }
    }

}
