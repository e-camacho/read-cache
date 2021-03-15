using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ReadCache.Controllers
{
    /// <summary>
    /// Forwards requests not matching other controllers to Configured proxy url
    /// </summary>
    [Route("{*url}", Order = 999)]
    public class ProxyController : Controller
    {
        private readonly ILogger logger;

        public ProxyController(ILogger logger)
        {
            this.logger = logger;
        }

        // Get verb for proxy route
        [HttpGet]
        public async Task<string> Get()
        {
            return await HttpHelper.GetData(new Uri(Configuration.ProxyUri, $"{this.Request.Path}{this.Request.QueryString}"), this.logger);
        }
    }
}
