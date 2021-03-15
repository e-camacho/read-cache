using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReadCache.Cache;

namespace ReadCache.Models
{
    /// <summary>
    /// Base implementation for collection or service response being cached
    /// </summary>
    public abstract class CacheableEntity
    {
        protected string cacheKey;
        protected ICache cache;
        protected ILogger logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
        /// <param name="cache">Cache handler</param>
        /// <param name="logger">Logging handler</param>
        public CacheableEntity(string cacheKey, ICache cache, ILogger logger)
        {
            this.cacheKey = cacheKey;
            this.cache = cache;
            this.logger = logger;
        }

        /// <summary>
        /// Retrieves entity value
        /// </summary>
        /// <returns>Entity value</returns>
        protected object GetValue()
        {
            return this.cache.GetValue(this.cacheKey);
        }

        /// <summary>
        /// Stores entity value on cache
        /// </summary>
        /// <param name="value">Entity value</param>
        protected void SetValue(object value)
        {
            this.cache.SetValue(this.cacheKey, value);
        }
    }
}
