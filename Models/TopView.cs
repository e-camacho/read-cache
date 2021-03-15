using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ReadCache.Cache;

namespace ReadCache.Models
{
    /// <summary>
    /// Provides a Top N view for a particular Endpoint data
    /// </summary>
    /// <typeparam name="T">Type for response tuple second member</typeparam>
    public class TopView<T> : CacheableEntity
    {
        #region TopEntry definition
        private class TopEntry<VT>
        {
            public string Key { get; set; }
            public VT Value { get; set; }

            public string GetFormattedValue()
            {
                if (this.Value is DateTime)
                {
                    DateTime value = Convert.ToDateTime(this.Value, System.Globalization.CultureInfo.InvariantCulture);
                    return string.Format("\"{0:s}Z\"", value);
                }
                else
                {
                    return this.Value.ToString();
                }
            }
        }
        #endregion

        private string pivot;
        private string identifier;
        private Endpoint endpoint;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
        /// <param name="pivot">Column name (from API JSON response) to use as criteria for top N descending sort</param>
        /// <param name="identifier">Column name (from API JSON response) to use as response tuple first member</param>
        /// <param name="endpoint">Endpoint object containing data to use for view construction</param>
        /// <param name="cache">Cache handler</param>
        /// <param name="logger">Logging handler</param>
        public TopView(string cacheKey, string pivot, string identifier, Endpoint endpoint, ICache cache, ILogger logger) : base(cacheKey, cache, logger)
        {
            this.pivot = pivot;
            this.identifier = identifier;
            this.endpoint = endpoint;
        }

        /// <summary>
        /// Returns a Top N results by pivot
        /// </summary>
        /// <param name="top">How many tuples to return</param>
        /// <returns>View response</returns>
        public string GetTop(int top)
        {
            const string responseFormat = "[{0}\n]";
            List<TopEntry<T>> entries = null;

            if (top <= 0)
            {
                return string.Format(responseFormat, string.Empty);
            }

            object data = this.GetValue();
            if (null == data)
            {
                // View has not cached data, grab cached data from Endpoint and build view
                entries = this.BuildAndCache();
            }
            else
            {
                entries = (List<TopEntry<T>>)data;
            }

            int adjustedTop = Math.Min(top, entries.Count);
            return string.Format(
                responseFormat, 
                string.Join(',', entries.Take(adjustedTop).Select(entry => $"\n        [\"{entry.Key}\",{entry.GetFormattedValue()}]")));
        }

        private List<TopEntry<T>> BuildAndCache()
        {
            try
            {
                string sourceData = this.endpoint.GetData();
                JArray jsonData = JArray.Parse(sourceData);
                List<TopEntry<T>> cacheValue = jsonData.Select(t => new TopEntry<T> { Key = t[this.identifier].ToString(), Value = t[this.pivot].Value<T>() }).OrderByDescending(e => e.Value).ToList();
                this.SetValue(cacheValue);
                return cacheValue;
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Exception building and caching View data");
                return new List<TopEntry<T>>();
            }
        }
    }
}
