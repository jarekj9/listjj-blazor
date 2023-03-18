using Listjj_frontend.Data;
using Listjj_frontend.Repository;
using Listjj_frontend.Services.Abstract;

namespace Listjj_frontend.Services
{
    public class UnitOfWork: IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext context;
        private readonly IServiceProvider serviceProvider;
        public IUserRepository Users { get; }

        public UnitOfWork(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            this.context = context;
            this.Users = new UserRepository(this.context);
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
