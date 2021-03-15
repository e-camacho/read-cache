using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caching=Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Memory;

namespace ReadCache.Cache
{
    /// <summary>
    /// Cache implementation using IMemoryCache
    /// </summary>
    public class MemoryCache : CacheBase
    {
        private Caching.IMemoryCache cache;

        /// <inheritdoc/>
        public MemoryCache(int expirationTimeInSeconds) : base(expirationTimeInSeconds)
        {
            this.cache = new Caching.MemoryCache(new Caching.MemoryCacheOptions());
        }

        /// <summary>
        /// Retrieves a value from Cache
        /// </summary>
        /// <param name="key">Cache Key</param>
        /// <returns>Value when found, null otherwise</returns>
        public override object GetValue(string key)
        {
            return this.cache.TryGetValue(key, out object value) ? value : null;
        }

        /// <inheritdoc/>
        public override void SetValue(string key, object value)
        {
            var entry = this.cache.CreateEntry(key);
            this.cache.Set(key, value, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(this.expirationTimeInSeconds) });
        }
    }
}
