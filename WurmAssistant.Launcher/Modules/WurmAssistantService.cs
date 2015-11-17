using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AldursLab.WurmAssistant.Launcher.Contracts;
using AldursLab.WurmAssistant.Shared.Dtos;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant.Launcher.Modules
{
    public class WurmAssistantService : IWurmAssistantService
    {
        readonly string webServiceRootUrl;
        readonly JsonSerializer serializer = new JsonSerializer();

        static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

        public WurmAssistantService([NotNull] string webServiceRootUrl)
        {
            if (webServiceRootUrl == null) throw new ArgumentNullException("webServiceRootUrl");
            if (webServiceRootUrl.EndsWith("/"))
            {
                webServiceRootUrl = webServiceRootUrl.Substring(0, webServiceRootUrl.Length - 1);
            }
            this.webServiceRootUrl = webServiceRootUrl;
        }

        public async Task<string> GetLatestVersionAsync(IProgressReporter progressReporter, string buildCode)
        {
            HttpClient client = new HttpClient();
            client.Timeout = DefaultTimeout;
            progressReporter.SetProgressPercent(null);
            progressReporter.SetProgressStatus("Obtaining latest version for build " + buildCode);
            var response =
                await client.GetAsync(string.Format("{0}/LatestBuildNumber/{1}", webServiceRootUrl, buildCode));
            if (response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var versionString = serializer.Deserialize<string>(new JsonTextReader(new StreamReader(stream)));
                    progressReporter.SetProgressStatus("Latest version for build " + buildCode + " is: " + (versionString ?? "None"));
                    return versionString;
                }
            }
            else
            {
                throw new ServiceException(string.Format("Server returned {0} : {1}",
                    response.StatusCode,
                    response.ReasonPhrase));
            }
        }

        public async Task<Package[]> GetAllPackages()
        {
            HttpClient client = new HttpClient();
            client.Timeout = DefaultTimeout;
            var response =
                await client.GetAsync(string.Format("{0}/Packages", webServiceRootUrl));
            if (response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var packages = serializer.Deserialize<Package[]>(new JsonTextReader(new StreamReader(stream)));
                    return packages;
                }
            }
            else
            {
                throw new ServiceException(string.Format("Server returned {0} : {1}",
                    response.StatusCode,
                    response.ReasonPhrase));
            }
        }
    }
}