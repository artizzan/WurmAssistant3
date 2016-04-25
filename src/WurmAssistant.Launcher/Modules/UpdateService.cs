using System;
using System.Threading.Tasks;
using AldursLab.Networking;
using AldursLab.WurmAssistant.Launcher.Contracts;
using AldursLab.WurmAssistant.Launcher.Dto;
using AldursLab.WurmAssistant.Shared.Dtos;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Modules
{
    public class UpdateService : IUpdateService
    {
        readonly string webServiceRootUrl;
        readonly IStagingLocation stagingLocation;
        readonly IWurmAssistantService wurmAssistantService;

        static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

        public UpdateService([NotNull] ControllerConfig controllerConfig, IStagingLocation stagingLocation)
        {
            if (controllerConfig == null) throw new ArgumentNullException("controllerConfig");
            if (stagingLocation == null) throw new ArgumentNullException("stagingLocation");

            this.webServiceRootUrl = controllerConfig.WebServiceRootUrl;
            if (webServiceRootUrl.EndsWith("/"))
            {
                webServiceRootUrl = webServiceRootUrl.Substring(0, webServiceRootUrl.Length - 1);
            }

            this.stagingLocation = stagingLocation;
            this.wurmAssistantService = new WurmAssistantService(webServiceRootUrl);
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

        public async Task<string> GetLatestVersionAsync(IProgressReporter progressReporter, string buildCode)
        {
            return await wurmAssistantService.GetLatestVersionAsync(progressReporter, buildCode);
        }

        public async Task<Package[]> GetAllPackages()
        {
            return await wurmAssistantService.GetAllPackages();
        }

        public async Task<string> GetCurrentUpdateSourceHost()
        {
            return await wurmAssistantService.GetCurrentUpdateSourceHost();
        }
    }
}