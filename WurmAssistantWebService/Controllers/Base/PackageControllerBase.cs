using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using AldursLab.WurmAssistantWebService.Model;
using AldursLab.WurmAssistantWebService.Model.Entities;
using AldursLab.WurmAssistantWebService.Model.Services;

namespace AldursLab.WurmAssistantWebService.Controllers.Base
{
    public abstract class PackageControllerBase : ApiController
    {
        protected readonly ApplicationDbContext Context = new ApplicationDbContext();
        protected readonly Files Files = new Files();

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

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(Files.Read(package.File.FileId));
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = package.File.Name
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
                var name = string.Format("{0}_{1}_{2}.7z",
                    projectType,
                    releaseType,
                    unescapedVersionString);

                Guid fileId;
                using (var contentStream = await Request.Content.ReadAsStreamAsync())
                {
                    fileId = Files.Create(name, contentStream);
                }

                var newFile = Context.Files.Single(file => file.FileId == fileId);

                var package = new WurmAssistantPackage()
                {
                    ProjectType = projectType,
                    ReleaseType = releaseType,
                    VersionString = unescapedVersionString,
                    File = newFile
                };
                Context.WurmAssistantPackages.Add(package);

                RemoveOutdatedPackages();
                Context.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        protected void RemoveOutdatedPackages()
        {
            var packageCount = Context.WurmAssistantPackages.Count();
            var toRemove = packageCount - 3;
            if (toRemove > 0)
            {
                var toDelete =
                    Context.WurmAssistantPackages.OrderByDescending(package => package.Created).Take(toRemove).ToArray();
                foreach (var package in toDelete)
                {
                    Context.WurmAssistantPackages.Remove(package);
                    Context.SaveChanges();
                    Files.Delete(package.FileId);
                }
            }
        }
    }
}