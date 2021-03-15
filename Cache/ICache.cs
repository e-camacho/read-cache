using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadCache.Cache
{
    /// <summary>
    /// Defines contract for retrieving and setting values in cache
    /// </summary>
    public interface ICache
    {
        object GetValue(string key);

        void SetValue(string key, object value);
    }
}
