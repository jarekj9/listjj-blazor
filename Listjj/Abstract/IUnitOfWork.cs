using System;
using System.Threading.Tasks;

namespace Listjj.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IListItemRepository ListItems { get; }
        ICategoryRepository Categories { get; }
        TRepository Repository<TRepository>();
        Task Save();
    }
}
