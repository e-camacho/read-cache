using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using ReadCache.Cache;
using ReadCache.Models;
using Microsoft.Extensions.Logging;

namespace ReadCache
{
    /// <summary>
    /// Acs as Manager for retrieving and caching data
    /// </summary>
    public class Repository
    {
        private MemoryCache memoryCache;
        private List<Endpoint> endpoints;
        private Timer refreshTimer;
        private ILogger logger;

        /// <summary>
        /// True if all endpoints are populated
        /// </summary>
        public bool IsHydrated { get; private set; }

        #region Cached Endpoints
        public Endpoint Root { get; private set; }
        public Endpoint Netflix { get; private set; }
        public Endpoint Members { get; private set; }
        public Endpoint Repos { get; private set; }
        #endregion

        #region Custom Views
        public TopView<int> TopNForks { get; private set; }
        public TopView<DateTime> TopNLastUpdated { get; private set; }
        public TopView<int> TopNOpenIssues { get; private set; }
        public TopView<int> TopNStars { get; private set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Repository()
        {
            this.memoryCache = new MemoryCache(Configuration.RefreshFrequencyInSeconds * 2);
        }

        /// <summary>
        /// Creates endpoints and views. Also does initial first population
        /// </summary>
        /// <param name="logger"></param>
        public void Initialize(ILogger logger)
        {
            this.logger = logger;
            this.Root = new Endpoint("Root", new Uri(Configuration.ProxyUri, "/"), this.memoryCache, logger);
            this.Netflix = new Endpoint("Netflix", new Uri(Configuration.ProxyUri, "orgs/Netflix"), this.memoryCache, logger);
            this.Repos = new Endpoint("Repos", new Uri(Configuration.ProxyUri, "orgs/Netflix/repos"), this.memoryCache, logger, 100);
            this.Members = new Endpoint("Members", new Uri(Configuration.ProxyUri, "orgs/Netflix/members"), this.memoryCache, logger);
            this.endpoints = new List<Endpoint> { this.Root, this.Netflix, this.Members, this.Repos };
#pragma warning disable CS4014 // Hydrate is fire & forget call
            this.Hydrate();
            this.TopNForks = new TopView<int>("TopNForks", "forks", "full_name", this.Repos, this.memoryCache, logger);
            this.TopNLastUpdated = new TopView<DateTime>("TopNLastUpdated", "updated_at", "full_name", this.Repos, this.memoryCache, logger);
            this.TopNOpenIssues = new TopView<int>("TopNOpenIssues", "open_issues", "full_name", this.Repos, this.memoryCache, logger);
            this.TopNStars = new TopView<int>("TopNStars", "stargazers_count", "full_name", this.Repos, this.memoryCache, logger);
        }

        private async Task Hydrate()
        {
            this.logger.LogInformation("Initial hydration started");

            if (await this.PopulateEndpoints())
            {
                this.logger.LogInformation("Repository is hydrated");
            }

            // Schedule periodic data refresh
            this.refreshTimer = new Timer(Configuration.RefreshFrequencyInSeconds * 1000);
            this.refreshTimer.Elapsed += async (sender, e) => await this.PopulateEndpoints();
            this.refreshTimer.AutoReset = true;
            this.refreshTimer.Enabled = true;
        }

        public async Task<bool> PopulateEndpoints()
        {
            List<Task<string>> tasks = this.endpoints.Select(e => e.Populate()).ToList();

            // Trigger views population as tasks in parallel. Note there are differences between tasks and threads
            await Task.WhenAll(tasks);
            bool success = !tasks.Any(task => string.IsNullOrWhiteSpace(task.Result));
            if (success)
            {
                this.IsHydrated = true;
            }
            else
            {
                // When Data population for at least one endpoint failed we don't consider repository as hydrated and /healthcheck will report 503
                this.logger.LogError("At least one Repository failed to populate");
                this.IsHydrated = false;
            }

            return success;
        }
    }
}