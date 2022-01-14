using Listjj.Abstract;
using Listjj.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using Listjj.Repository;
using System.Threading.Tasks;

namespace Listjj.Transaction
{
    public class UnitOfWork: IUnitOfWork, IDisposable
    {
        private readonly AppDbContext context;
        private readonly IServiceProvider serviceProvider;
        public IUserRepository Users { get; }
        public IListItemRepository ListItems { get; }
        public ICategoryRepository Categories { get; }

        public UnitOfWork(AppDbContext context, IServiceProvider serviceProvider)
        {
            this.context = context;
            this.Users = new UserRepository(this.context);
            this.Categories = new CategoryRepository(this.context);
            this.ListItems = new ListItemRepository(this.context);
            this.serviceProvider = serviceProvider;
        }

        public TRepository Repository<TRepository>()
        {
            var repository = this.serviceProvider.GetService<TRepository>();
            return (TRepository)repository;
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }

    }
}
