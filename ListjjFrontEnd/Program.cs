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

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

// General app services
builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddScoped<AppState>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ITagsService, TagsService>();


builder.Services.AddMudServices();

await builder.Build().RunAsync();
