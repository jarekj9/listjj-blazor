using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Listjj.Abstract;
using List.Extensions;
using Microsoft.JSInterop;

namespace Listjj.Service
{
    public class CategoryCacheService : ICategoryCacheService
    {
        private readonly IDistributedCache cache;
        private readonly string recentCategoryKey;
        private readonly IUnitOfWork unitOfWork;
        private readonly IJSRuntime jSRuntime;
        private Guid userId;
        public CategoryCacheService(IDistributedCache cache, IUnitOfWork unitOfWork, IJSRuntime jSRuntime)
        {
            this.cache = cache;
            this.recentCategoryKey = "RecentCategory:";
            this.unitOfWork = unitOfWork;
            this.jSRuntime = jSRuntime;
            this.userId = Guid.Empty;
        }

        public Task<Guid> GetRecentCategoryAsync(Guid userId)
        {
            this.userId = userId;
            return cache.GetRecordAsync<Guid>($"{recentCategoryKey}{userId}", Load);
        }

        private async Task<Guid> Load()
        {
            var categories = await unitOfWork.Categories.GetAllByUserId(userId);
            var recentCategoryIdCookie = await jSRuntime.InvokeAsync<string>("ReadCookie", "recent_category_id");
            if (Guid.TryParse(recentCategoryIdCookie, out var recentCategoryId) && categories != null)
            {
                bool categoryExists = categories.Exists(x => x.Id == recentCategoryId);
                if (categoryExists)
                {
                    return recentCategoryId;
                }
            }
            return Guid.Empty;
        }
            
        
        public async Task UpdateRecentCategoryCache(Guid userId, Guid recentCategory)
        {
            await cache.SetRecordAsync($"{recentCategoryKey}{userId}", recentCategory);
        }
    }
}
