using System;
using System.Threading.Tasks;

namespace Listjj.Abstract
{
    public interface ICategoryCacheService
    {
        Task<Guid> GetRecentCategoryAsync(Guid userId);
        Task UpdateRecentCategoryCache(Guid userId, Guid recentCategory);
    }
}
