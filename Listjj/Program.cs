using Listjj.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Listjj;
using System;


var builder = WebApplication.CreateBuilder(args);
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);
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



startup.Configure(app, app.Environment);

app.MapRazorPages();
app.Run();
