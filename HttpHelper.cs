using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace ReadCache
{
    /// <summary>
    /// Provides functionality for issuing Web requests
    /// </summary>
    public class HttpHelper
    {
        private static readonly string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.74 Safari/537.36 Edg/79.0.309.43";
        private static readonly Regex linkRegexMatch = new Regex("<(.*?)>; rel=\"next\"", RegexOptions.Compiled);

        public static async Task<string> GetData(Uri url, ILogger logger, uint requestPageSize = 0)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(HttpHelper.UserAgent);

            // Set Authorization Token if available
            if (!string.IsNullOrWhiteSpace(Configuration.AuthorizationToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token"/*"Bearer"*/, Configuration.AuthorizationToken);
            }

            string content = string.Empty;
            bool success = false;
            if (requestPageSize == 0)
            {
                // Single request
                (content, success, _) = await HttpHelper.SendWebRequest(httpClient, url, 3, logger);
            }
            else
            {
                // Use paging
                List<JArray> partialResults = new List<JArray>();
                Uri requestUrl = new Uri(url, $"?page=1&per_page={requestPageSize}");
                do
                {
                    (content, success, requestUrl) = await HttpHelper.SendWebRequest(httpClient, requestUrl, 3, logger, true);
                    if (success)
                    {
                        try
                        {
                            partialResults.Add(JArray.Parse(content));
                            if (null == requestUrl)
                            {
                                // This was last page, merge and serialize final result
                                JArray concatenatedResults = new JArray(partialResults.SelectMany(r => r));
                                content = concatenatedResults.ToString();
                            }
                        }
                        catch (Exception e)
                        {
                            logger.LogWarning(e, "Unable to deserialize json response");
                            success = false;
                        }
                    }
                } while (!string.IsNullOrWhiteSpace(content) && (null != requestUrl) && success);
            }

            // Return result from web call(s), except in the case where incomplete results or errors
            return success ? content : string.Empty;
        }

        #region GetData auxiliary functions
        private static async Task<(string, bool, Uri)> SendWebRequest(HttpClient client, Uri url, uint retries, ILogger logger, bool lookForPageLink = false)
        {
            uint leftRetries = retries;
            while (leftRetries > 0)
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    logger.LogInformation($"Sent web request for url: {url}");
                    string content = await response.Content.ReadAsStringAsync();
                    int statusCode = (int)response.StatusCode;
                    if (statusCode >= 400)
                    {
                        logger.LogWarning($"Unsuccessful web request for url: {url}, http status code {statusCode}, response: {content}");
                        break;
                    }

                    return (content, true, lookForPageLink ? HttpHelper.GetNextPageUrl(response) : null);
                }
                catch (Exception e)
                {
                    if ((e is WebException) && ((WebException)e).Status == WebExceptionStatus.Timeout)
                    {
                        leftRetries--;
                        logger.LogWarning($"Timeout while sending web request for url (retries left {leftRetries}): {url}");
                        continue;
                    }

                    logger.LogError(e, $"Exception while sending web request for url: {url}");
                    leftRetries = 0;
                    break;
                }
            }

            return (string.Empty, false, null);
        }

        private static Uri GetNextPageUrl(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Link", out IEnumerable<string> linkHeader))
            {
                if (linkHeader.Any())
                {
                    string[] links = linkHeader.First().Split(',');
                    foreach (string link in links)
                    {
                        MatchCollection matches = linkRegexMatch.Matches(link);
                        if (matches.Count > 0)
                        {
                            // Url is on Group[1] when expression matches
                            return new Uri(matches[0].Groups[1].Value);
                        }
                    }
                }
            }

            return null;
        }
        #endregion
    }
}
