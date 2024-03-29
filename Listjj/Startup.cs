using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

using Listjj.Data;
using Listjj.Service;
using Listjj.Abstract;
using Listjj.Models;

// Auth:
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.HttpOverrides;
using Listjj.Repository;
using Listjj.Transaction;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.OpenApi.Models;

namespace Listjj
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            //using Microsoft.EntityFrameworkCore:
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("SqlDbContext"), new MySqlServerVersion(new Version(10, 5, 9))
                )
            );
            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

            //Redis Caching
            services.AddStackExchangeRedisCache(opt => opt.Configuration = Configuration.GetConnectionString("Redis"));

            services.AddHttpContextAccessor();  // for user identity
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IRefreshService, RefreshService>();
            services.AddScoped<ITagsCacheService, TagsCacheService>();

            services.AddScoped<UserManager<ApplicationUser>>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryCacheService, CategoryCacheService>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IListItemRepository, ListItemRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<AppState>();

            //monitoring number of users connections
            services.AddSingleton<CircuitHandler, CircuitHandlerService>();

            //mud blazor
            services.AddMudServices();

            //for APIs:
            services.AddMvc(setupAction: options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Listjj", Version = "v1" });
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
            });

            // login with google:
            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // to use nginx headers and have correct google login redirect url (https):
            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
                RequireHeaderSymmetry = false
            };
            forwardedHeadersOptions.KnownNetworks.Clear();
            forwardedHeadersOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardedHeadersOptions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //Auth:
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //Auth:
                endpoints.MapControllers();

                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            //for APIs:
            app.UseMvcWithDefaultRoute();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blazor API V1");
            });

            // DB Migration:
            // using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            // {
            //     var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
            //     context.Database.Migrate();
            // }
        }
    }

}
