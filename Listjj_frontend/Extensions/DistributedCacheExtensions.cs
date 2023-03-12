using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Listjj_frontend.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetRecordAsync<T>(
            this IDistributedCache cache,
            string recordKey,  // key in Redis
            T data,  // stored value
            TimeSpan? absoluteExpireTime = null,
            TimeSpan? unusedExpireTime = null
        )
        {
            var options = new DistributedCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(36000000);
            options.SlidingExpiration = unusedExpireTime;

            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordKey, jsonData, options);
        }

        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordKey)
        {
            var jsonData = await cache.GetStringAsync(recordKey);

            if (jsonData is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }

        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordKey, Func<Task<T>> valueFunc)
        {
            var result = await cache.GetRecordAsync<T>(recordKey);

            if (result != null)
            {
                return result;
            }
            var value = await valueFunc();
            await cache.SetRecordAsync(recordKey, value);

            return value;
        }

    }
}