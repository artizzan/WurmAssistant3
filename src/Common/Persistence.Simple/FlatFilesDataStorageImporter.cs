using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldursLab.Essentials.FileSystem;
using AldursLab.Essentials.Synchronization;

namespace AldursLab.Persistence.Simple
{
    public class FlatFilesDataStorageImporter
    {
        readonly string rootPath;

        public FlatFilesDataStorageImporter(string rootPath)
        {
            this.rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
        }

        internal IEnumerable<RawDataObject> GetAllObjects()
        {
            return new DirectoryInfo(rootPath)
                   .GetDirectories()
                   .Select(info => new {Directory = info, Files = info.GetFiles()})
                   .SelectMany(arg =>
                   {
                       return arg.Files.Select(info =>
                       {
                           var setId = arg.Directory.Name;
                           var objectId = Path.GetFileNameWithoutExtension(info.Name);
                           var data = TryLoad(setId, objectId);
                           return new RawDataObject(setId, objectId, data);
                       });
                   });
        }

        string TryLoad(string setId, string objectId)
        {
            var fullPath = MakeFilePath(setId, objectId);
            var data = ReliableFileOps.TryReadFileContents(fullPath);
            return data;
        }

        string MakeFilePath(string storagePath, string objectId)
        {
            return Path.Combine(rootPath, storagePath, objectId) + ".dat";
        }
    }
}