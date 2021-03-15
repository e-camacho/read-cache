# API Read Cache

## Summary
This ASP .Net Core application provides a cross-platform solution for read caching for some GitHub APIs.

## Features
1. Provides a read cache for Github service endpoints below, reducing underlining calls to Github API:
  * /
  * /orgs/Netflix
  * /orgs/Netflix/members
  * /orgs/Netflix/repos
2. Implements following custom views for [Netflix organization](https://github.com/Netflix) repositories:

Endpoint | Description
-------- | -----------
/view/top/N/forks | Top-N repos by number of forks
/view/top/N/last_updated | Top-N repos by last updated time
/view/top/N/open_issues | Top-N repos by open issues
/view/top/N/stars Top-N | repos by stars

3. Proxies API endpoints ouside the two lists above through the service to Github API
4. Provides a /healthcheck endpoint to determine service readiness to serve requests
## Design
### High level
When application is launched then the following happens:
1. Read configuration values (from config file and command line parameter) required to spin service and start listening on configured port
2. Queries GitHub API (configurable url) to load data from endpoints, and it caches results (default 20 mins, configurable). Then, service is ready to serve incoming requests
3. Views are connected to Repos endpoint object. Views data is lazy loaded/cached on first request
4. Periodic data refresh from GitHub API (default 10 mins, configurable)
### Design decisions
1. Since Github API responses are just a couple of MBs size, memory cache is used for this implementation. Implemented abstraction easily allows to extend to other technologies like Redis
2. To speed up user requests processing, data is read from GitHub API endpoints and cached during service start up
3. To reduce initial overall cache size for views, view data collection is built and cached upon first view request. View cached data is significantly smaller than endpoint cached data, since only contains required information to serve its request
4. Data is cached using SlidingExpiration, allowing that on the remote case periodic refresh fails, endpoints can still serve content as long these are used before configured expiration window
5. Github API is queried using retries. When querying Github repos endpoint it uses pagination, this since org might have hundreds of repos
6. /healthcheck endpoint returns 200 when service is ready to serve requests, 503 otherwise
### Extensibility
* .Net Core ILogger provides out of the box functionality to customize logging consumers and filtering
* Caching functionality is abstracted in a way that could be easily extended to use other technologies, i.e.: Redis
* Implementation allows to easily have additional views and endpoints with just adding few lines
* Github API is configurable, in case Github API service url changes in the future
## Setup
### Prerequisites
#### Build and Run
Download and install **.NET Core 2.2 SDK** [Win](https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-2.2.207-windows-x64-installer) | [Mac](https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-2.2.207-macos-x64-installer)
#### Test
1. [Curl](https://curl.se/docs/manpage.html)
2. [jq](https://stedolan.github.io/jq/download/)

### Grab Source and Build
1. Clone repository or download ZIP and decompress into a local folder
2. Open shell (bash for Mac or Command Prompt for Windows)
3. Switch to **source directory** (where [ReadCache.csproj](https://github.com/e-camacho/read-cache/ReadCache.csproj) file is located)
4. Build solution using "dotnet build"

## Usage
### Configuration
Config file [launchSettings.json](https://github.com/e-camacho/read-cache/Properties/launchSettings.json) allows to customize following values (inside ReadCache element):
* **proxyServiceUrl:** Url to use as API data source, default is https://api.github.com
* **refreshFrequencyInSeconds:** How frequently (after initial population) data will be refreshed from data source, default is 10 mins
* **applicationPort:** Port to use when not provided as command parameter (see Execution)
* **GITHUB_API_TOKEN:** Add an element in config file to specify a token to avoid [Rate limiting](https://docs.github.com/en/rest/overview/resources-in-the-rest-api#rate-limiting) 
### Execution
Once solution is built (see Setup step 4) you can run application, optionally specifying port. Syntax: **dotnet run** *[-- port=<port number>]*
Here is an example:
`dotnet run -- port=1234` 
### Testing
While application is running, open a second shell, switch to **source directory** and execute your test script
