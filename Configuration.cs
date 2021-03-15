using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadCache
{
    /// <summary>
    /// Holds Configuration Settings
    /// </summary>
    public class Configuration
    {
        private static readonly Uri DefaultGitHubServiceUrl = new Uri("https://api.github.com");
        private const int DefaultRefreshFrequencyInSeconds = 300;
        private const int DefaultApplicationPort = 1414;

        private static Uri proxyUri;
        private static string authorizationToken;
        private static int refreshFrequencyInSeconds;
        private static int applicationPort;

        /// <summary>
        /// Proxy Service Url 
        /// </summary>
        public static Uri ProxyUri
        {
            get
            {
                if (null == Configuration.proxyUri)
                {
                    // Initialize proxy url from launchSettings.json
                    string proxyUrl = Environment.GetEnvironmentVariable("ProxyServiceUrl");
                    if (string.IsNullOrWhiteSpace(proxyUrl) || !Uri.TryCreate(proxyUrl, UriKind.Absolute, out Configuration.proxyUri))
                    {
                        // Assign default proxy since configuration value was either not found or invalid
                        Configuration.proxyUri = Configuration.DefaultGitHubServiceUrl;
                    }
                }

                return Configuration.proxyUri;
            }
        }

        /// <summary>
        /// AuthorizationToken from environment/configuration. Empty if not set
        /// </summary>
        public static string AuthorizationToken
        {
            get
            {
                if (null == Configuration.authorizationToken)
                {
                    // Initialize token from environment variables or launchSettings.json
                    string token = Environment.GetEnvironmentVariable("GITHUB_API_TOKEN");
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        // Assign default since configuration value was either not found
                        Configuration.authorizationToken = string.Empty;
                    }
                    else
                    {
                        Configuration.authorizationToken = token;
                    }
                }

                return Configuration.authorizationToken;
            }
        }

        /// <summary>
        /// How often cached responses are refreshed from source (in seconds)
        /// </summary>
        public static int RefreshFrequencyInSeconds
        {
            get
            {
                if (0 == Configuration.refreshFrequencyInSeconds)
                {
                    // Initialize value from environment variables or launchSettings.json
                    string cacheSetting = Environment.GetEnvironmentVariable("refreshFrequencyInSeconds");
                    if (string.IsNullOrWhiteSpace(cacheSetting)
                        || !Int32.TryParse(cacheSetting, out Configuration.refreshFrequencyInSeconds)
                        || Configuration.refreshFrequencyInSeconds < 0)
                    {
                        // Assign default refresh value since configuration value was either not found or invalid
                        Configuration.refreshFrequencyInSeconds = Configuration.DefaultRefreshFrequencyInSeconds;
                    }
                }

                return Configuration.refreshFrequencyInSeconds;
            }
        }

        /// <summary>
        /// Service port number to use when not provided from command line
        /// </summary>
        public static int ApplicationPort
        {
            get
            {
                if (0 == Configuration.applicationPort)
                {
                    // Initialize application port from environment variables or launchSettings.json
                    string cacheSetting = Environment.GetEnvironmentVariable("applicationPort");
                    if (string.IsNullOrWhiteSpace(cacheSetting)
                        || !Int32.TryParse(cacheSetting, out Configuration.applicationPort)
                        || Configuration.applicationPort < 0)
                    {
                        // Assign default value since configuration value was either not found or invalid
                        Configuration.applicationPort = Configuration.DefaultApplicationPort;
                    }
                }

                return Configuration.applicationPort;
            }
        }
    }
}
