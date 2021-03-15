using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ReadCache.Controllers
{
    /// <summary>
    /// Serves requests for a particular org
    /// </summary>
    [Route("orgs/[controller]")]
    public class NetflixController : Controller
    {
        private readonly Repository repository;

        public NetflixController(Repository repository)
        {
            this.repository = repository;
        }

        // GET orgs/Netflix
        [HttpGet]
        public string Get()
        {
            return this.repository.Netflix.GetData();
        }

        // GET orgs/Netflix/members
        [HttpGet("members")]
        public string GetMembers()
        {
            return this.repository.Members.GetData();
        }

        // GET orgs/Netflix/repos
        [HttpGet("repos")]
        public string GetRepos()
        {
            return this.repository.Repos.GetData();
        }
    }
}
