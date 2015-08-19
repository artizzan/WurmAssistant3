using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AldursLab.WurmAssistantWebService.Controllers.Base;
using AldursLab.WurmAssistantWebService.Model.Entities;

namespace AldursLab.WurmAssistantWebService.Controllers
{
    /// <summary>
    /// Web API for WurmAssistant3
    /// </summary>
    [RoutePrefix("api/WurmAssistant3")]
    public class WurmAssistant3Controller : PackageControllerBase
    {
        /// <summary>
        /// Gets latest version of Beta package or "0.0.0.0" if none available
        /// </summary>
        /// <returns>String representing version, eg "1.2.3.4"</returns>
        [Route("Beta/LatestVersion")]
        public string GetLatestBetaVersion()
        {
            return GetLatestVersion(ProjectType.WurmAssistantLite, ReleaseType.Beta);
        }

        /// <summary>
        /// Gets latest version of Stable package or "0.0.0.0" if none available
        /// </summary>
        /// <returns>String representing version, eg "1.2.3.4"</returns>
        [Route("Stable/LatestVersion")]
        public string GetLatestStableVersion()
        {
            return GetLatestVersion(ProjectType.WurmAssistantLite, ReleaseType.Stable);
        }

        /// <summary>
        /// Gets beta package for specified version.
        /// </summary>
        /// <param name="versionString">Escaped version string eg. "1-2-3-4" instead of "1.2.3.4"</param>
        /// <returns>mime multipart file (byte content of 7zip archive file)</returns>
        [Route("Beta/Package/{versionString}")]
        public HttpResponseMessage GetBetaPackage(string versionString)
        {
            return GetPackage(ProjectType.WurmAssistantLite, ReleaseType.Beta, versionString);
        }

        /// <summary>
        /// Gets stable package for specified version.
        /// </summary>
        /// <param name="versionString">Escaped version string eg. "1-2-3-4" instead of "1.2.3.4"</param>
        /// <returns>mime multipart file (byte content of 7zip archive file)</returns>
        [Route("Stable/Package/{versionString}")]
        public HttpResponseMessage GetStablePackage(string versionString)
        {
            return GetPackage(ProjectType.WurmAssistantLite, ReleaseType.Stable, versionString);
        }

        /// <summary>
        /// Posts new beta package. Content of the message should contain bytes of 7zip archive file.
        /// </summary>
        /// <param name="versionString">Escaped version string eg. "1-2-3-4" instead of "1.2.3.4"</param>
        /// <returns></returns>
        [Route("Beta/Package/{versionString}")]
        [Authorize(Roles = "Publish")]
        public async Task<HttpResponseMessage> PostBetaPackage(string versionString)
        {
            return await PostPackage(ProjectType.WurmAssistantLite, ReleaseType.Beta, versionString);
        }

        /// <summary>
        /// Posts new stable package. Content of the message should contain bytes of 7zip archive file.
        /// </summary>
        /// <param name="versionString">Escaped version string eg. "1-2-3-4" instead of "1.2.3.4"</param>
        /// <returns></returns>
        [Route("Stable/Package/{versionString}")]
        [Authorize(Roles = "Publish")]
        public async Task<HttpResponseMessage> PostStablePackage(string versionString)
        {
            return await PostPackage(ProjectType.WurmAssistantLite, ReleaseType.Stable, versionString);
        }
    }
}