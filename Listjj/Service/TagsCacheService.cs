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
        private IListItemRepository listItemRepository;
        private string TagsSelectionKey = "TagsSelection";
        private Guid tagsUserId = Guid.Empty;
        public TagsCacheService(IDistributedCache cache, IListItemRepository ListItemRepository)
        {
            this.cache = cache;
            this.listItemRepository = ListItemRepository;
        }

        public Task<List<string>> GetTagsSelectionAsync(Guid userId)
        {
            tagsUserId = userId;
            TagsSelectionKey = $"TagsSelection:{userId}";
            return cache.GetRecordAsync<List<string>>(TagsSelectionKey, Load);
        }

        private async Task<List<string>> Load()
        {
            var alltags = new List<String>();
            var listItems = await this.listItemRepository.GetAllByUserId(tagsUserId);
            alltags.AddRange(listItems.SelectMany(i => i.Tags?.Split(',')).Distinct().ToList());
            return alltags;
        }

        public async Task UpdateCache(Guid userId, List<string> tagsSelection)
        {
            var tagsSelectionKey = $"TagsSelection:{userId}";
            await cache.SetRecordAsync(tagsSelectionKey, tagsSelection);
        }
    }
}
