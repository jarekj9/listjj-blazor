using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Listjj.Models;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

namespace Listjj.Data
{
    public class AppDbContext: IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<ListItem> ListItems{ get; set; }   // table name ListItems
        public DbSet<Category> Categories{ get; set; }
        public DbSet<File> Files{ get; set; }

        // to use roles:**
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationRole>().HasData(new ApplicationRole { Name = "User", NormalizedName = "USER", Id = Guid.NewGuid(), ConcurrencyStamp = Guid.NewGuid().ToString() });
            builder.Entity<ApplicationRole>().HasData(new ApplicationRole { Name = "Admin", NormalizedName = "ADMIN", Id = Guid.NewGuid(), ConcurrencyStamp = Guid.NewGuid().ToString() });

            builder.Entity<ListItem>()
                .HasMany(i => i.Files)
                .WithOne(f => f.ListItem)
                .HasForeignKey(f => f.ListItemId);
        }
    }

    public class DbContextFactory<TContext> 
        : IDbContextFactory<TContext> where TContext : DbContext
    {
        private readonly IServiceProvider provider;

        public DbContextFactory(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public TContext CreateDbContext()
        {
            if (provider == null)
            {
                throw new InvalidOperationException(
                    $"You must configure an instance of IServiceProvider");
            }

            return ActivatorUtilities.CreateInstance<TContext>(provider);
        }
    }
}