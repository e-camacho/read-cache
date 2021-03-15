using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ReadCache
{
    /// <summary>
    /// App entry point
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .UseStartup<Startup>()
                .UseUrls(
                    string.Format(
                        "http://localhost:{0}/",
                        ((null != args) && args.Any(a => a.ToLowerInvariant().StartsWith("port="))) 
                            ? args.First(a => a.ToLowerInvariant().StartsWith("port=")).Split('=')[1] 
                        : Configuration.ApplicationPort.ToString()
                        ))
                .Build();
    }
}
