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

namespace AldursLab.WurmAssistantWebService.Controllers
{
    /// <summary>
    /// Web API for WurmAssistant3
    /// </summary>
    [RoutePrefix("api/WurmAssistant3")]
    public class WurmAssistant3Controller : ApiController
    {
        protected readonly ApplicationDbContext Context = new ApplicationDbContext();
        protected readonly Files Files = new Files();
        protected readonly Logs Logs = new Logs();

        /// <summary>
        /// Gets build number of the latest available package for specified build definition.
        /// </summary>
        /// <param name="buildCode">Code representing build definition. Case insensitive.</param>
        /// <returns>String representing build number. Is not guaranteed to be numeric. Empty string if no packages found.</returns>
        [Route("LatestBuildNumber/{buildCode}")]
        public string GetLatestBuildNumber(string buildCode)
        {
            var latest = (from p in Context.WurmAssistantPackages
                            where p.BuildCode == buildCode
                            orderby p.Created descending
                            select p).FirstOrDefault();

            return latest == null ? string.Empty : latest.BuildNumber;
        }

        /// <summary>
        /// Gets package file for specified build code and build number.
        /// </summary>
        /// <param name="buildCode">Code representing build definition. Case insensitive.</param>
        /// <param name="buildNumber">String representing build number. Case insensitive.</param>
        /// <returns>Single-part mime-multipart file response, containing byte content of the file and original file name.</returns>
        [Route("Packages/{buildCode}/{buildNumber}")]
        public HttpResponseMessage GetPackage(string buildCode, string buildNumber)
        {
            var package = (from p in Context.WurmAssistantPackages
                           where p.BuildCode == buildCode
                                 && p.BuildNumber == buildNumber
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

        /// <summary>
        /// Posts new package. Content of the message should be single part mime-multipart file upload 
        /// with disposition containing desired file name.
        /// </summary>
        /// <param name="buildCode">Code representing build definition. Case insensitive.</param>
        /// <param name="buildNumber">String representing build number. Case insensitive.</param>
        /// <returns></returns>
        [Route("Packages/{buildCode}/{buildNumber}")]
        [Authorize(Roles = "Publish")]
        public async Task<HttpResponseMessage> PostPackage(string buildCode, string buildNumber)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType,
                    "Content must be mime multipart");
            }

            if (string.IsNullOrWhiteSpace(buildCode))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = "Build code cannot be empty"
                };
            }
            if (string.IsNullOrWhiteSpace(buildNumber))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = "Build number cannot be empty"
                };
            }

            try
            {
                var existingPackage =
                    Context.WurmAssistantPackages
                           .SingleOrDefault(
                               assistantPackage =>
                                   assistantPackage.BuildCode == buildCode
                                   && assistantPackage.BuildNumber == buildNumber);

                if (existingPackage != null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        ReasonPhrase =
                            string.Format("Package with build code {0} and build number {1} already exists",
                                buildCode,
                                buildNumber)
                    };
                }

                var name = Request.Content.Headers.ContentDisposition.FileName;

                Guid fileId;
                using (var contentStream = await Request.Content.ReadAsStreamAsync())
                {
                    fileId = Files.Create(name, contentStream);
                }

                var newFile = Context.Files.Single(file => file.FileId == fileId);

                var package = new WurmAssistantPackage()
                {
                    BuildCode = buildCode,
                    BuildNumber = buildNumber,
                    File = newFile
                };
                Context.WurmAssistantPackages.Add(package);

                RemoveOutdatedPackages(buildCode);
                Context.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        void RemoveOutdatedPackages(string buildCode)
        {
            try
            {
                var allPackages =
                    Context.WurmAssistantPackages.Where(
                        package => package.BuildCode == buildCode).ToArray();

                var packageCount = allPackages.Count();
                var countToRemove = packageCount - 3;
                if (countToRemove > 0)
                {
                    var toDelete = allPackages.OrderBy(package => package.Created).Take(countToRemove);
                    foreach (var package in toDelete)
                    {
                        Context.WurmAssistantPackages.Remove(package);
                        Context.SaveChanges();
                        Files.Delete(package.FileId);
                    }
                }
            }
            catch (Exception exception)
            {
                Logs.Add("WurmAssistant3Controller.RemoveOutdatedPackages", "Error: " + exception.ToString());
            }
        }
    }
}