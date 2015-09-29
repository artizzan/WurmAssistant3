using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AldursLab.Networking;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant.Launcher.Core
{
    public interface IWurmAssistantService
    {
        Task<string> GetLatestVersionAsync(IProgressReporter progressReporter, string buildCode);

        Task<IStagedPackage> GetPackageAsync(IProgressReporter progressReporter, string buildCode, string buildNumber);
    }

    public class WurmAssistantService : IWurmAssistantService
    {
        readonly string webServiceRootUrl;
        readonly IStagingLocation stagingLocation;

        readonly JsonSerializer serializer = new JsonSerializer();

        static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

        public WurmAssistantService([NotNull] ControllerConfig controllerConfig, IStagingLocation stagingLocation)
        {
            if (controllerConfig == null) throw new ArgumentNullException("controllerConfig");
            if (stagingLocation == null) throw new ArgumentNullException("stagingLocation");
            this.webServiceRootUrl = controllerConfig.WebServiceRootUrl;
            if (webServiceRootUrl.EndsWith("/"))
            {
                webServiceRootUrl = webServiceRootUrl.Substring(0, webServiceRootUrl.Length - 1);
            }

            this.stagingLocation = stagingLocation;
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
                    progressReporter.SetProgressStatus("Latest version for build " + buildCode + " is: " + versionString);
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

        public async Task<IStagedPackage> GetPackageAsync(IProgressReporter progressReporter, string buildCode,
            string buildNumber)
        {
            progressReporter.SetProgressPercent(0);
            progressReporter.SetProgressStatus("Downloading package for build " + buildCode + " for version "
                                               + buildNumber);

            var tempFile = stagingLocation.CreateTempFile();

            using (var webclient = new ExtendedWebClient((int) DefaultTimeout.TotalMilliseconds))
            {
                var tcs = new TaskCompletionSource<bool>();
                byte lastPercent = 0;
                webclient.DownloadProgressChanged += (sender, args) =>
                {
                    var percent = (byte) (((double) args.BytesReceived/(double) args.TotalBytesToReceive)*100);
                    if (percent > lastPercent)
                    {
                        lastPercent = percent;
                        progressReporter.SetProgressPercent(percent);
                        progressReporter.SetProgressStatus(string.Format("Downloaded {0}/{1}",
                            args.BytesReceived,
                            args.TotalBytesToReceive));
                    }
                };
                webclient.DownloadFileCompleted += (sender, args) =>
                {
                    if (args.Error != null)
                    {
                        progressReporter.SetProgressPercent(100);
                        progressReporter.SetProgressStatus("download error: " + args.Error.ToString());
                        tcs.SetException(new ServiceException("Download error", args.Error));
                    }
                    else
                    {
                        progressReporter.SetProgressStatus("download completed");
                        tcs.SetResult(true);
                    }
                };
                webclient.DownloadFileAsync(
                    new Uri(string.Format("{0}/Packages/{1}/{2}", webServiceRootUrl, buildCode, buildNumber)),
                    tempFile.FullName);

                await tcs.Task;

                return stagingLocation.CreatePackageFromZipFile(tempFile.FullName);
            }

        }
    }

    [Serializable]
    public class ServiceException : Exception
    {
        public ServiceException()
        {
        }

        public ServiceException(string message) : base(message)
        {
        }

        public ServiceException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ServiceException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
