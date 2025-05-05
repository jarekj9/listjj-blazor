using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using System.Net.Security;
using System.Security.Authentication;

using Listjj.Data;
using Listjj.Service;
using Listjj.Abstract;
using Listjj.Models;
using Listjj.Consumers;
using Listjj.Infrastructure.Events;

using StackExchange.Redis;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

// Auth:
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.HttpOverrides;
using Listjj.Repository;
using Listjj.Transaction;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using Microsoft.Extensions.Options;
using Listjj.Data.Options;
using Ganss.Xss;
using Listjj.Middleware;


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

            ///// Open Telemetry /////
            var openTelemetryEntpoint = Configuration.GetSection("OpenTelemetryEndpoint").Get<string>();
            if (!string.IsNullOrEmpty(openTelemetryEntpoint))
            { 
                services.AddOpenTelemetry().WithTracing(b =>
                {
                    b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Listjj api"))
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(opts => { opts.Endpoint = new Uri(openTelemetryEntpoint); });
                });
            }
            //AddOpenTelemetryExtension("Listjj api");
            //////////////////////////


            // CORS for Blazor WASM integration
            var origins = Configuration.GetSection("CorsOrigins").Get<string[]>();
            if (origins?.Any() ?? false)
            {
                services.AddCors(policy =>
                {
                    policy.AddPolicy("CORSOrigins", builder =>
                       builder.WithOrigins(origins)
                      .SetIsOriginAllowed((host) => true)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials());
                });
            }
            else
            {
                services.AddCors(policy =>
                {
                    policy.AddPolicy("CORSOrigins", builder =>
                       builder.WithOrigins("*")
                      .SetIsOriginAllowed((host) => true)
                      .AllowAnyMethod()
                      .AllowAnyHeader());
                });
            }


            // Use mariadb or mssql, remember to change migrations
            var mysqlConnString = Configuration.GetConnectionString("MySqlDbContext");
            var mssqlConnString = Configuration.GetConnectionString("MsSqlDbContext");
            if(!string.IsNullOrEmpty(mysqlConnString))
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseMySql(mysqlConnString, new MySqlServerVersion(new Version(10, 5, 9)))
                );
            }
            else
            {
                services.AddDbContext<AppDbContext>(options => options.UseSqlServer(mssqlConnString));
            }

            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

            //Redis Caching:
            var redisConnString = Configuration.GetConnectionString("Redis");
            if (!string.IsNullOrEmpty(redisConnString))
            {
                services.AddStackExchangeRedisCache(opt =>
                {
                    var configurationOptions = ConfigurationOptions.Parse(redisConnString);
                    opt.ConfigurationOptions = configurationOptions;
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            services.AddSingleton<HtmlSanitizer>();
            services.Configure<GoogleAuthOptions>(Configuration.GetSection($"Authentication:{nameof(GoogleAuthOptions)}"));
            services.Configure<MicrosoftAuthOptions>(Configuration.GetSection($"Authentication:{nameof(MicrosoftAuthOptions)}"));

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

            //for APIs:
            services.AddMvc(setupAction: options => options.EnableEndpointRouting = false);
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
                googleOptions.ClientId = Configuration[$"Authentication:{nameof(GoogleAuthOptions)}:ClientId"];
                googleOptions.ClientSecret = Configuration[$"Authentication:{nameof(GoogleAuthOptions)}:ClientSecret"];
            });

            // enable JWT:
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidAudience = Configuration["JwtAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecurityKey"]))
                    };
                });

            ////////////////////////// Masstransit TLS
            if (Configuration.GetSection("RabbitMqTlsConfig").Value != null)
            { 
                services.AddScoped<IProducerService, ProducerService>();
                var queuePrefix = "listjj";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                services.AddMassTransit(config =>
                {
                    config.AddConsumer<TestEventConsumer>();
                    config.UsingRabbitMq((context, cfg) =>
                    {
                        config.AddConsumer<TestEventConsumer>();
                        var rabbitMqTlsConfig = Configuration.GetSection("RabbitMqTlsConfig");
                        if (rabbitMqTlsConfig.GetValue<bool>("UseTls"))
                        {
                            cfg.Host(new Uri(rabbitMqTlsConfig["RabbitMqRootUri"]), h =>
                            {
                                h.Username(rabbitMqTlsConfig["UserName"]);
                                h.Password(rabbitMqTlsConfig["Password"]);
                                h.UseSsl(s =>
                                {
                                    s.Protocol = SslProtocols.Tls12;
                                    s.ServerName = rabbitMqTlsConfig["ServerCertCommonName"];
                                    s.AllowPolicyErrors(SslPolicyErrors.RemoteCertificateChainErrors);
                                    s.Certificate = new X509Certificate2(
                                        rabbitMqTlsConfig["ClientCertPath"], rabbitMqTlsConfig["ClientCertPassword"],
                                        X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable
                                    );
                                });
                            });
                        }
                        else
                        {
                            cfg.Host(new Uri(rabbitMqTlsConfig["RabbitMqRootUri"]), h =>
                            {
                                h.Username(rabbitMqTlsConfig["UserName"]);
                                h.Password(rabbitMqTlsConfig["Password"]);
                            });
                        }
                        cfg.ReceiveEndpoint($"{queuePrefix}_{nameof(TestEvent)}", c => { c.ConfigureConsumer<TestEventConsumer>(context); });
                    });
                });
            }
            //////////////////////////


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

            //app.UseHttpsRedirection(); // unknown CORS issue if enabled
            app.UseStaticFiles();

            app.UseRouting();

            //Auth:
            app.UseCors("CORSOrigins");

            app.UseMiddleware<TokenToHeaderMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
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
        }
    }

}
