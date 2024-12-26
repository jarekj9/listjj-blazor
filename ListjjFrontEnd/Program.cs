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
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
var configuration = builder.Configuration.Build();
var appSettings = configuration.Get<AppSettings>();


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
