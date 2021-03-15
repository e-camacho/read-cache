using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadCache.Cache
{
    /// <summary>
    /// Base class for caching values
    /// Child classes might implement specific caching technologies, like Memory Cache or Redis
    /// </summary>
    public abstract class CacheBase: ICache
    {
        protected int expirationTimeInSeconds;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="expirationTimeInSeconds">Time to use for cache item expiration</param>
        public CacheBase(int expirationTimeInSeconds)
        {
            this.expirationTimeInSeconds = expirationTimeInSeconds;
        }

        /// <summary>
        /// Retrieves a value from Cache
        /// </summary>
        /// <param name="key">Cache Key</param>
        /// <returns>Value</returns>
        public abstract object GetValue(string key);

        /// <summary>
        /// Adds a cache entry
        /// </summary>
        /// <param name="key">Cache Key</param>
        /// <param name="value">Value to store</param>
        public abstract void SetValue(string key, object value);
    }
}
