using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Listjj.Models;
using System.Collections.Generic;


namespace Listjj.Abstract
{
    public interface ITagsCacheService
    {
        Task<List<string>> GetTagsSelectionAsync(string userId);
        Task LoadToCache();
        Task UpdateCache(string userId, List<string> tagsSelection);
    }
}
