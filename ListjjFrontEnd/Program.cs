using Blazored.LocalStorage;
using ListjjFrontEnd;
using ListjjFrontEnd.Data;
using ListjjFrontEnd.Services;
using ListjjFrontEnd.Services.Abstract;
using ListjjFrontEnd.Services.Abstract.Authentication;
using ListjjFrontEnd.Services.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System.Net.Http.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//var environment = builder.HostEnvironment.BaseAddress;
//AppSettings? appSettings = new AppSettings();
//if (environment == "Production")
//{
//    var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
//    appSettings = await httpClient.GetFromJsonAsync<AppSettings>($"appsettings.{environment}.json");
//}
//else
//{
//    appSettings = builder.Configuration.Get<AppSettings>();
//}
//if (appSettings == null)
//{
//    throw new Exception("appSettings must be initialized before registering services.");
//}

//var configBuild = new ConfigurationBuilder();
//var configuration = configBuild.AddEnvironmentVariables().Build();
//var oldApps = builder.Configuration.Get<AppSettings>();


builder.Configuration.AddEnvironmentVariables();
var appSettings = builder.Configuration.Get<AppSettings>();
//var appServiceApiEndpoint = builder.Configuration.GetValue<string>("APPSETTING_ApiEndpoint");


builder.Services.AddSingleton<AppSettings>(appSettings);
builder.Services.AddScoped(sp => new HttpClient());
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

// General app services
builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddScoped<AppState>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ITagsService, TagsService>();
builder.Services.AddScoped<IExternalAccessApiService, ExternalAccessApiService>();


builder.Services.AddMudServices();

await builder.Build().RunAsync();
