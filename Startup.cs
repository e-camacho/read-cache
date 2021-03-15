using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ReadCache
{
    /// <summary>
    /// Application Initialization functionality
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Used to setup additional services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(typeof(Repository));
            services.AddLogging();
            services.AddSingleton(typeof(ILogger), typeof(Logger<Repository>));
        }

        /// <summary>
        /// This method gets called by the runtime. Used to configure the HTTP request pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="repository">Data repository</param>
        /// <param name="logger">Logging handler</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Repository repository, ILogger logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            repository.Initialize(logger);
        }
    }
}
