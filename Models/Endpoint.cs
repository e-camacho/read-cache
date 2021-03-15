using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReadCache.Cache;

namespace ReadCache.Models
{
    /// <summary>
    /// Allows querying, caching and retrieving (value) for an API endpoint (data source)
    /// </summary>
    public class Endpoint : CacheableEntity
    {
        private Uri dataSource;
        private uint requestPageSize;

        /// <summary>
        /// Endpoint constructor
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
        /// <param name="dataSource">Url for API to query</param>
        /// <param name="cache">Cache handler</param>
        /// <param name="logger">Logging handler</param>
        /// <param name="requestPageSize">Web response page size, zero if no paging is used</param>
        public Endpoint(string cacheKey, Uri dataSource, ICache cache, ILogger logger, uint requestPageSize = 0) : base(cacheKey, cache, logger)
        {
            this.dataSource = dataSource;
            this.requestPageSize = requestPageSize;
        }

        /// <summary>
        /// Queries from data source and stores in cache
        /// </summary>
        /// <returns>API response</returns>
        public async Task<string> Populate()
        {
            string data = await this.LoadData();
            this.SetValue(data);
            return data;
        }

        /// <summary>
        /// Retrieves cached value
        /// </summary>
        /// <returns>Cached value</returns>
        public string GetData()
        {
            return (string)this.GetValue();
        }

        private async Task<string> LoadData()
        {
            string data = await HttpHelper.GetData(this.dataSource, this.logger, requestPageSize);
            return data;
        }
    }
}
