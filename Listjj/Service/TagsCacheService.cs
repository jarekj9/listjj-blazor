using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Listjj.Abstract;
using List.Extensions;

namespace Listjj.Service
{
    public class TagsCacheService : ITagsCacheService
    {
        private readonly IDistributedCache cache;
        private IListItemService listItemService;
        private string TagsSelectionKey = "TagsSelection";
        private string tagsUserId = String.Empty;
        public TagsCacheService(IDistributedCache cache, IListItemService ListItemService)
        {
            this.cache = cache;
            this.listItemService = ListItemService;
        }

        public async Task LoadToCache()
        {
            var tagsSelection = await Load();
            await cache.SetRecordAsync(TagsSelectionKey, tagsSelection);
        }

        public async Task ClearCache() => cache.RemoveAsync(TagsSelectionKey);

        public Task<List<string>> GetTagsSelectionAsync(string userId)
        {
            tagsUserId = userId;
            TagsSelectionKey = $"TagsSelection:{userId}";
            return cache.GetRecordAsync<List<string>>(TagsSelectionKey, Load);
        }

        private async Task<List<string>> Load()
        {
            var alltags = new List<String>();
            var listItems = await this.listItemService.GetItemsByUserId(tagsUserId);
            alltags.AddRange(listItems.SelectMany(i => i.Tags?.Split(',')).Distinct().ToList());
            return alltags;
        }
        public async Task UpdateCache(string userId, List<string> tagsSelection)
        {
            var tagsSelectionKey = $"TagsSelection:{userId}";
            await cache.SetRecordAsync(tagsSelectionKey, tagsSelection);
        }
    }
}
