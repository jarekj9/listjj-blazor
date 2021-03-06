using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Listjj.Abstract
{
    public interface ITagsCacheService
    {
        Task<List<string>> GetTagsSelectionAsync(Guid userId);
        Task UpdateCache(Guid userId, List<string> tagsSelection);
    }
}
