using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using AldursLab.WurmAssistantWebService.Model;
using AldursLab.WurmAssistantWebService.Model.Entities;

namespace AldursLab.WurmAssistantWebService.Controllers.Base
{
    public abstract class PackageControllerBase : ApiController
    {
        protected readonly ApplicationDbContext Context = new ApplicationDbContext();

        protected string GetLatestVersion(ProjectType projectType, ReleaseType releaseType)
        {
            var packages = (from p in Context.WurmAssistantPackages
                            where p.ProjectType == projectType
                                  && p.ReleaseType == releaseType
                            orderby p.Created descending
                            select p).ToArray();

            var latest = (from p in packages
                          orderby p.Version descending, p.Created descending
                          select p).FirstOrDefault();

            if (latest == null)
            {
                return new Version(0,0,0,0).ToString();
            }
            else
            {
                return latest.VersionString;
            }
        }

        protected HttpResponseMessage GetPackage(ProjectType projectType, ReleaseType releaseType, string versionString)
        {
            string unescapedVersionString = versionString.Replace("-", ".");
            var package = (from p in Context.WurmAssistantPackages
                           where p.VersionString == unescapedVersionString
                                 && p.ProjectType == projectType
                                 && p.ReleaseType == releaseType
                           orderby p.Created descending
                           select p).FirstOrDefault();
            if (package == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            var fileContents = package.File.Contents;
            var fileName = package.File.CombinedName;

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new System.IO.MemoryStream(fileContents));
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            return response;
        }

        protected async Task<HttpResponseMessage> PostPackage(ProjectType projectType, ReleaseType releaseType,
            string versionString)
        {
            string unescapedVersionString = versionString.Replace("-", ".");
            Version version;
            if (!Version.TryParse(unescapedVersionString, out version))
            {
                return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType,
                    "versionString is not valid: " + versionString);
            }
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, 
                    "Content must be mime multipart");
            }

            try
            {
                // Read the form data.
                var content = await Request.Content.ReadAsByteArrayAsync();
                var file = new File()
                {
                    Name =
                        string.Format("WurmAssistant{0}_{1}_{2}",
                            projectType,
                            releaseType,
                            unescapedVersionString),
                    Extension = "7z",
                    Contents = content
                };
                var package = new WurmAssistantPackage()
                {
                    ProjectType = projectType,
                    ReleaseType = releaseType,
                    VersionString = unescapedVersionString,
                    File = file
                };
                Context.WurmAssistantPackages.Add(package);
                await RemoveOutdatedPackages();
                await Context.SaveChangesAsync();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        protected async Task RemoveOutdatedPackages()
        {
            var toDelete =
                Context.WurmAssistantPackages.Where(
                    package => package.Created < DateTimeOffset.Now.Subtract(TimeSpan.FromDays(1)));
            Context.WurmAssistantPackages.RemoveRange(toDelete);
        }
    }
}