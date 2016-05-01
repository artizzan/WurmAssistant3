using System;
using System.IO;
using AldursLab.Essentials.FileSystem;
using JetBrains.Annotations;

namespace AldursLab.PersistentObjects.Persistence
{
    public class FlatFilesPersistenceStrategy : IPersistenceStrategy
    {
        readonly DataMap dataMap;

        public FlatFilesPersistenceStrategy([NotNull] PersistenceManagerConfig config)
        {
            if (config == null) throw new ArgumentNullException("config");

            dataMap = new DataMap(config.DataStoreDirectoryPath);
        }

        public string TryLoad(string collectionId, string key)
        {
            var filepath = dataMap.GetFileFullPathForObject(collectionId, key);
            var data = ReliableFileOps.TryReadFileContents(filepath);
            return data;
        }

        public void Save(string collectionId, string key, string content)
        {
            var filepath = dataMap.GetFileFullPathForObject(collectionId, key);
            ReliableFileOps.SaveFileContents(filepath, content);
        }

        public void TryDeleteData(string collectionId, string key)
        {
            var filepath = dataMap.GetFileFullPathForObject(collectionId, key);
            var info = new FileInfo(filepath);
            if (info.Exists)
            {
                info.Delete();
            }
            dataMap.DeleteMapping(collectionId, key);
        }
    }
}