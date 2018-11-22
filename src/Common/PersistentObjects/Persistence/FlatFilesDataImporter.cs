using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.Essentials.FileSystem;
using JetBrains.Annotations;

namespace AldursLab.PersistentObjects.Persistence
{
    public class FlatFilesDataImporter
    {
        readonly DataMap dataMap;

        public FlatFilesDataImporter([NotNull] PersistenceManagerConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            dataMap = new DataMap(config.DataStoreDirectoryPath);
        }

        public IEnumerable<RawDataObject> GetAllObjects() =>
            dataMap.GetAllCollections()
                   .SelectMany(collectionId =>
                   {
                       return dataMap
                              .GetAllKeys(collectionId)
                              .Select(key =>
                                  new RawDataObject(collectionId,
                                      key,
                                      TryLoad(collectionId, key)));
                   });

        string TryLoad(string collectionId, string key)
        {
            var filepath = dataMap.GetFileFullPathForObject(collectionId, key);
            var data = ReliableFileOps.TryReadFileContents(filepath);
            return data;
        }
    }
}