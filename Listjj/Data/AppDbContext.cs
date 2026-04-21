using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Listjj.Models;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

            builder.Entity<ApplicationRole>().HasData(new ApplicationRole { Name = "User", NormalizedName = "USER", Id = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), ConcurrencyStamp = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567111").ToString() });
            builder.Entity<ApplicationRole>().HasData(new ApplicationRole { Name = "Admin", NormalizedName = "ADMIN", Id = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567891"), ConcurrencyStamp = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567222").ToString() });

            builder.Entity<ListItem>()
                .HasMany(i => i.Files)
                .WithOne(f => f.ListItem)
                .HasForeignKey(f => f.ListItemId);

            // Convert all DateTime properties to UTC automatically
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                }
            }
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