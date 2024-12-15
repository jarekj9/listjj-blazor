using Blazored.LocalStorage;
using ListjjFrontEnd.Services.Abstract.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Listjj.Infrastructure.Models;
using Listjj.Infrastructure.Data;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using ListjjFrontEnd.Data;
using System.Net.Http;

namespace ListjjFrontEnd.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly AppSettings _appsettings;
        private readonly string apiEndpoint;
        private readonly NavigationManager _navigationManager;

        public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, NavigationManager navigationManager,
            ILocalStorageService localStorage, AppSettings appsettings)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _navigationManager = navigationManager;
            _localStorage = localStorage;
            _appsettings = appsettings;
            _httpClient.BaseAddress = new Uri("https://dupa"); 
            apiEndpoint = "https://dupa";
        }

        public async Task<RegisterResult> Register(RegisterModel registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync<RegisterModel>($"{apiEndpoint}/api/accounts/register", registerModel);
            var result = await response.Content.ReadFromJsonAsync<RegisterResult>();
            return result;
        }

        public async Task<LoginResult> Login(LoginModel loginModel)
        {
            var loginAsJson = JsonSerializer.Serialize(loginModel);
            var response = await _httpClient.PostAsync($"{apiEndpoint}/api/Login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
            var loginResult = JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!response.IsSuccessStatusCode)
            {
                return loginResult;
            }

            await _localStorage.SetItemAsync("authToken", loginResult.Token);
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.Email);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

            return loginResult;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }


        [JSInvokable]
        public async Task GoogleLogin(GoogleResponse googleResponse)
        {
            var principal = new ClaimsPrincipal();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(googleResponse.Credential) as JwtSecurityToken;
            var email = jsonToken?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = jsonToken?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var loginResult = await Login(new LoginModel() { Email = email, Password = "WillBeIgnored123", GoogleJwt = googleResponse.Credential });
            if(loginResult.Successful)
            {
                _navigationManager.NavigateTo("/list");
            }
            else
            {
                _navigationManager.NavigateTo("/login");
            }
        }
    }
}
