using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization;
using AldursLab.Networking;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant.LauncherCore
{
    public interface IWurmAssistantService
    {
        Task<Version> GetLatestVersionAsync(IProgressReporter progressReporter);

        Task<IStagedPackage> GetPackageAsync(IProgressReporter progressReporter, Version version);
    }

    public class WurmAssistantService : IWurmAssistantService
    {
        readonly string webServiceRootUrl;
        readonly IStagingLocation stagingLocation;

        readonly JsonSerializer serializer = new JsonSerializer();

        // expected Web API:
        // http://url/LatestVersion returns string
        // http://url/Package/[version] where version is Version returns File

        public WurmAssistantService(string webServiceRootUrl, IStagingLocation stagingLocation)
        {
            if (webServiceRootUrl == null) throw new ArgumentNullException("webServiceRootUrl");
            if (stagingLocation == null) throw new ArgumentNullException("stagingLocation");
            this.webServiceRootUrl = webServiceRootUrl;
            this.stagingLocation = stagingLocation;
        }

        public async Task<Version> GetLatestVersionAsync(IProgressReporter progressReporter)
        {
            HttpClient client = new HttpClient();
            progressReporter.SetProgressPercent(null);
            progressReporter.SetProgressStatus("Obtaining latest version");
            var response = await client.GetAsync(webServiceRootUrl + "/LatestVersion");
            if (!response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var versionString = serializer.Deserialize<string>(new JsonTextReader(new StreamReader(stream)));
                    progressReporter.SetProgressStatus("Latest version is: " + versionString);
                    return new Version(versionString);
                }
            }
            else
            {
                throw new ServiceException(string.Format("Server returned {0} : {1}",
                    response.StatusCode,
                    response.ReasonPhrase));
            }
        }

        public async Task<IStagedPackage> GetPackageAsync(IProgressReporter progressReporter, Version version)
        {
            progressReporter.SetProgressPercent(0);
            progressReporter.SetProgressStatus("Downloading package for version " + version);

            var tempFile = stagingLocation.CreateTempFile();
            try
            {
                using (var webclient = new ExtendedWebClient())
                {
                    var tcs = new TaskCompletionSource<bool>();
                    byte lastPercent = 0;
                    webclient.DownloadProgressChanged += (sender, args) =>
                    {
                        var percent = (byte) (args.BytesReceived/args.TotalBytesToReceive);
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
                        new Uri(webServiceRootUrl + "/Package/" + version.ToString().Replace(".", "-")),
                        tempFile.FullName);

                    await tcs.Task;

                    return stagingLocation.CreatePackageFromSevenZipByteArray(File.ReadAllBytes(tempFile.FullName), version);
                }
            }
            finally
            {
                tempFile.Delete();
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
