using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AldursLab.WurmAssistantWebService.Model.Entities;

namespace AldursLab.WurmAssistantWebService.Model.Services
{
    public class Files
    {
        static readonly object GlobalSync = new object();

        readonly DirectoryInfo filesDirectory;
        readonly ApplicationDbContext context = new ApplicationDbContext();

        public Files()
        {
            var filesDirPath = HttpContext.Current.Server.MapPath("~/App_Data/Files");
            filesDirectory = new DirectoryInfo(filesDirPath);

            // avoid creating dir at the same time by multiple requests, would fail a request with exception
            if (!filesDirectory.Exists)
            {
                lock (GlobalSync)
                {
                    if (!filesDirectory.Exists)
                    {
                        filesDirectory.Create();
                    }
                }
            }
        }

        public Guid Create(string nameWithExtension, Stream fileContents)
        {
            var newStorageName = Guid.NewGuid().ToString() + ".dat";
            var storagePath = BuildPath(newStorageName);
            using (Stream s = System.IO.File.OpenWrite(storagePath))
            {
                fileContents.CopyTo(s);
            }

            var file = new Entities.File()
            {
                Name = nameWithExtension,
                StorageFileName = newStorageName
            };

            context.Files.Add(file);
            context.SaveChanges();

            return file.FileId;
        }

        public Stream Read(Guid fileId)
        {
            var dbFile = context.Files.SingleOrDefault(f => f.FileId == fileId);
            if (dbFile == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return System.IO.File.OpenRead(BuildPath(dbFile.StorageFileName));
        }

        public void Delete(Guid fileId)
        {
            var dbFile = context.Files.SingleOrDefault(f => f.FileId == fileId);
            if (dbFile != null)
            {
                context.Files.Remove(dbFile);
                context.SaveChanges();

                var storagePath = BuildPath(dbFile.StorageFileName);
                var file = new FileInfo(storagePath);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
        }

        string BuildPath(string newStorageName)
        {
            return Path.Combine(filesDirectory.FullName, newStorageName);
        }
    }
}