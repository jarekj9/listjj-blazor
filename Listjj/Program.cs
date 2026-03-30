using Ganss.Xss;
using Listjj.Abstract;
using Listjj.Consumers;
using Listjj.Data;
using Listjj.Data.Options;
using Listjj.Infrastructure.Events;
using Listjj.Models;
using Listjj.Repository;
using Listjj.Service;
using Listjj.Transaction;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
// Auth:
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

///// Open Telemetry /////
var openTelemetryEntpoint = builder.Configuration.GetSection("OpenTelemetryEndpoint").Get<string>();
if (!string.IsNullOrEmpty(openTelemetryEntpoint))
{
    builder.Services.AddOpenTelemetry().WithTracing(b =>
    {
        b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Listjj api"))
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter(opts => { opts.Endpoint = new Uri(openTelemetryEntpoint); });
    });
}
//AddOpenTelemetryExtension("Listjj api");
//////////////////////////


// CORS for Blazor WASM integration
var origins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>();
if (origins?.Any() ?? false)
{
    builder.Services.AddCors(policy =>
    {
        policy.AddPolicy("CORSOrigins", b =>
           b.WithOrigins(origins)
          .SetIsOriginAllowed((host) => true)
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials());
    });
}
else
{
    builder.Services.AddCors(policy =>
    {
        policy.AddPolicy("CORSOrigins", b =>
           b.WithOrigins("*")
          .SetIsOriginAllowed((host) => true)
          .AllowAnyMethod()
          .AllowAnyHeader());
    });
}


// Use mssql or pgsql, remember to change migrations
var mssqlConnString = builder.Configuration.GetConnectionString("MsSqlDbContext");
var pgsqlConnString = builder.Configuration.GetConnectionString("PgSqlDbContext");

if (!string.IsNullOrEmpty(mssqlConnString))
{
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(mssqlConnString));
}
else if (!string.IsNullOrEmpty(pgsqlConnString))
{
    builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(pgsqlConnString));
}
else
{
    throw new Exception("No database connection string configured.");
}

builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

//Redis Caching:
var redisConnString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnString))
{
    builder.Services.AddStackExchangeRedisCache(opt =>
    {
        var configurationOptions = ConfigurationOptions.Parse(redisConnString);
        opt.ConfigurationOptions = configurationOptions;
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

builder.Services.AddSingleton<HtmlSanitizer>();
builder.Services.Configure<GoogleAuthOptions>(builder.Configuration.GetSection($"Authentication:{nameof(GoogleAuthOptions)}"));
builder.Services.Configure<MicrosoftAuthOptions>(builder.Configuration.GetSection($"Authentication:{nameof(MicrosoftAuthOptions)}"));

builder.Services.AddHttpContextAccessor();  // for user identity
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IRefreshService, RefreshService>();
builder.Services.AddScoped<ITagsCacheService, TagsCacheService>();

builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryCacheService, CategoryCacheService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IListItemRepository, ListItemRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<AppState>();

//monitoring number of users connections
builder.Services.AddSingleton<CircuitHandler, CircuitHandlerService>();

//for APIs:
builder.Services.AddMvc();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Listjj", Version = "v1" });
//});
builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(typeof(ListjjMappingProfile).Assembly);

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
});

// login with google:
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration[$"Authentication:{nameof(GoogleAuthOptions)}:ClientId"];
    googleOptions.ClientSecret = builder.Configuration[$"Authentication:{nameof(GoogleAuthOptions)}:ClientSecret"];
});

// enable JWT:
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtIssuer"],
            ValidAudience = builder.Configuration["JwtAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSecurityKey"]))
        };
    });

var app = builder.Build();

// creates DB and applies migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}

// to use nginx headers and have correct google login redirect url (https):
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
    RequireHeaderSymmetry = false
};
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection(); // unknown CORS issue if enabled
app.UseStaticFiles();

app.UseRouting();

//Auth:
app.UseCors("CORSOrigins");

app.UseAuthentication();
app.UseAuthorization();

try
{
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapBlazorHub();
        endpoints.MapRazorPages();
        endpoints.MapFallbackToPage("/_Host");
    });
}
catch (Exception ex) when (ex is ReflectionTypeLoadException rtle)
{
    foreach (var loaderEx in rtle.LoaderExceptions)
        Console.WriteLine(loaderEx?.Message);
    throw;
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();  // for Scalar.AspNetCore, adds /scalar api reference page
}


app.Run();