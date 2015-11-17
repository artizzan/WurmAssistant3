using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using AldursLab.WurmAssistant.Shared.Dtos;
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
        /// Gets info about all available packages.
        /// </summary>
        /// <returns></returns>
        [Route("Packages")]
        public Package[] GetAllPackages()
        {
            var result = Context.WurmAssistantPackages.Select(
                package => new Package()
                {
                    WurmAssistantPackageId = package.WurmAssistantPackageId,
                    BuildCode = package.BuildCode,
                    BuildNumber = package.BuildNumber
                });
            return result.ToArray();
        }

        /// <summary>
        /// Deletes a package.
        /// </summary>
        /// <param name="id"></param>
        [Route("Packages/{id}")]
        [Authorize(Roles = "Admin")]
        public void DeletePackage(Guid id)
        {
            var package =
                Context.WurmAssistantPackages.SingleOrDefault(
                    assistantPackage => assistantPackage.WurmAssistantPackageId == id);

            if (package == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            Context.WurmAssistantPackages.Remove(package);
            Context.SaveChanges();
            Files.Delete(package.FileId);
        }

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

            var parts = await Request.Content.ReadAsMultipartAsync();
            if (parts.Contents.Count > 1)
            {
                throw new InvalidOperationException("Expected single part in multipart, actual: " + parts.Contents.Count);
            }
            var firstContent = parts.Contents.First();
            var fileName = firstContent.Headers.ContentDisposition.FileName;

            Guid fileId;
            using (var contentStream = await firstContent.ReadAsStreamAsync())
            {
                fileId = Files.Create(fileName, contentStream);
            }

            var newFile = Context.Files.Single(file => file.FileId == fileId);

            var package = new WurmAssistantPackage()
            {
                BuildCode = buildCode,
                BuildNumber = buildNumber,
                File = newFile
            };
            Context.WurmAssistantPackages.Add(package);
            Context.SaveChanges();

            RemoveOutdatedPackages(buildCode);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        void RemoveOutdatedPackages(string buildCode)
        {
            if (buildCode.StartsWith("beta", StringComparison.InvariantCultureIgnoreCase)
                || buildCode.StartsWith("stable", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            try
            {
                var allPackages =
                    Context.WurmAssistantPackages.Where(
                        package => package.BuildCode == buildCode).ToArray();

                var packageCount = allPackages.Count();
                var countToRemove = packageCount - 10;
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