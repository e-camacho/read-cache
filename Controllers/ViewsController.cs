using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ReadCache.Controllers
{
    /// <summary>
    /// Serves requests for Views
    /// </summary>
    [Route("view/top")]
    public class ViewsController : Controller
    {
        private readonly Repository repository;

        public ViewsController(Repository repository)
        {
            this.repository = repository;
        }

        // GET view/top/N/forks
        [HttpGet("{count}/forks")]
        public string GetTopNForks(int count)
        {
            return this.repository.TopNForks.GetTop(count);
        }

        // GET view/top/N/last_updated
        [HttpGet("{count}/last_updated")]
        public string GetTopNLastUpdated(int count)
        {
            return this.repository.TopNLastUpdated.GetTop(count);
        }

        // GET view/top/N/open_issues
        [HttpGet("{count}/open_issues")]
        public string GetTopNOpenIssues(int count)
        {
            return this.repository.TopNOpenIssues.GetTop(count);
        }

        // GET view/top/N/stars
        [HttpGet("{count}/stars")]
        public string GetTopNStars(int count)
        {
            return this.repository.TopNStars.GetTop(count);
        }
    }
}
