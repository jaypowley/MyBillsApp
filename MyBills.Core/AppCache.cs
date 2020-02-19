using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MyBills.Core
{
    public static class AppCache<TItem>
    {
        private static readonly IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
        private static readonly ConcurrentDictionary<object, SemaphoreSlim> Locks = new ConcurrentDictionary<object, SemaphoreSlim>();

        /// <summary>
        /// Gets or creates a new cached item.
        /// </summary>
        /// <param name="key">The key to get or create in cache</param>
        /// <param name="createItem">The item to get or create in cache</param>
        /// <returns></returns>
        public static async Task<TItem> GetOrCreate(object key, Func<Task<TItem>> createItem)
        {
            if (!Cache.TryGetValue(key, out TItem cacheEntry))
            {
                var mylock = Locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
 
                await mylock.WaitAsync();
                try
                {
                    if (!Cache.TryGetValue(key, out cacheEntry))
                    {
                        // Key not in cache, so get data.
                        cacheEntry = await createItem();
                        Cache.Set(key, cacheEntry);
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            return cacheEntry;
        }
    }
}
