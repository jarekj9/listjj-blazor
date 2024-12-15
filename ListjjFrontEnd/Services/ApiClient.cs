using Blazored.LocalStorage;
using ListjjFrontEnd.Services.Abstract;
using ListjjFrontEnd.Services.Abstract.Authentication;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Microsoft.Extensions.Options;
using ListjjFrontEnd.Data;

namespace ListjjFrontEnd.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient httpClient;
        private readonly string apiEndpoint;
        private readonly ILocalStorageService localStorage;
        private readonly IAuthService authService;
        private readonly AppSettings _appsettings;

        public ApiClient(HttpClient httpClient, AppSettings appsettings, ILocalStorageService localStorage, IAuthService authService)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
            this.authService = authService;
            _appsettings = appsettings;
            apiEndpoint = _appsettings.ApiEndpoint;
        }

        public async Task<(TResponse Result, HttpResponseMessage HttpResponse)> Get<TResponse>(string urlPart)
        {
            await SetAuthToken();

            var response = await httpClient.GetAsync($"{apiEndpoint}{urlPart}");
            if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                authService.Logout();
            }
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
            if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                authService.Logout();
            }
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
