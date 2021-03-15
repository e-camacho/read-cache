using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadCache.Cache;

namespace ReadCache.Controllers
{
    /// <summary>
    /// Controller for root level operations, including healthcheck
    /// </summary>
    [Route("")]
    public class RootController : Controller
    {
        private readonly Repository repository;

        public RootController(Repository repository)
        {
            this.repository = repository;
        }

        // GET /
        [HttpGet]
        public string Get()
        {
            return this.repository.Root.GetData();
        }

        // GET /healthcheck
        // Returns 200 when service is ready to respond to requests, and 503 (Service Unavailable) when still loading up info
        [HttpGet("healthcheck")]
        public IActionResult HealthCheck()
        {
            return this.repository.IsHydrated ? Ok() : StatusCode(503);
        }
    }
}
