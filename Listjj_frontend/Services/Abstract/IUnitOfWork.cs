using System;
using System.Threading.Tasks;

namespace Listjj_frontend.Services.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        TRepository Repository<TRepository>();
        Task Save();
    }
}
