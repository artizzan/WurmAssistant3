using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AldursLab.Essentials.FileSystem;
using Newtonsoft.Json;

namespace AldursLab.PersistentObjects.Persistence
{
    public class DataMap
    {
        private readonly string dataStoreDirectoryFullPath;
        private const string DictionaryFileName = "data.map";

        private readonly string dataStoreDictionaryFileFullPath;

        private readonly Collections collections;

        readonly JsonSerializer serializer;

        public DataMap(string dataStoreDirectoryFullPath)
        {
            this.dataStoreDirectoryFullPath = dataStoreDirectoryFullPath;
            dataStoreDictionaryFileFullPath = Path.Combine(dataStoreDirectoryFullPath, DictionaryFileName);

            serializer = new JsonSerializer { Formatting = Formatting.Indented };

            var dictionaryFileContents = ReliableFileOps.TryReadFileContents(dataStoreDictionaryFileFullPath);
            if (!string.IsNullOrEmpty(dictionaryFileContents))
            {
                using (var sr = new StringReader(dictionaryFileContents))
                {
                    using (var jtr = new JsonTextReader(sr))
                    {
                        collections = serializer.Deserialize<Collections>(jtr);
                    }
                }
            }
            else
            {
                if (!Directory.Exists(dataStoreDirectoryFullPath))
                {
                    Directory.CreateDirectory(dataStoreDirectoryFullPath);
                }
                collections = new Collections();
            }
        }

        public string GetDirectoryFullPathForCollection(string collectionId)
        {
            Keys keys = GetKeys(collectionId);
            return Path.Combine(dataStoreDirectoryFullPath, keys.DirectoryName);
        }

        public string GetFileFullPathForObject(string collectionId, string key)
        {
            Keys keys = GetKeys(collectionId);

            string fileName;
            if (!keys.KeyToFileNameMap.TryGetValue(key, out fileName))
            {
                fileName = AddKeyToFileNameMapping(keys, key);
            }

            return Path.Combine(dataStoreDirectoryFullPath, keys.DirectoryName, fileName);
        }

        public IEnumerable<string> GetAllCollections()
        {
            return collections.CollectionIdToObjectsMap.Keys.ToList();
        }

        public IEnumerable<string> GetAllKeys(string collectionId)
        {
            Keys keys;
            if (collections.CollectionIdToObjectsMap.TryGetValue(collectionId, out keys))
            {
                return keys.KeyToFileNameMap.Keys.ToArray();
            }

            return new string[0];
        }

        public void DeleteMapping(string collectionId, string key)
        {
            Keys keys;
            if (collections.CollectionIdToObjectsMap.TryGetValue(collectionId, out keys))
            {
                keys.KeyToFileNameMap.Remove(key);
                Save();
            }
        }

        Keys GetKeys(string collectionId)
        {
            Keys keys;
            if (!collections.CollectionIdToObjectsMap.TryGetValue(collectionId, out keys))
            {
                keys = AddKeys(collectionId);
            }
            return keys;
        }

        private Keys AddKeys(string collectionId)
        {
            var entities = new Keys() { CollectionId = collectionId, DirectoryName = GenerateFolderNameForCollection(), };
            collections.CollectionIdToObjectsMap.Add(collectionId, entities);
            Save();
            return entities;
        }

        /// <summary>
        /// Returns file name
        /// </summary>
        private string AddKeyToFileNameMapping(Keys keys, string key)
        {
            var fileName = GenerateFileNameForKey(Path.Combine(dataStoreDirectoryFullPath, keys.DirectoryName));
            keys.KeyToFileNameMap.Add(
                key,
                fileName);
            Save();
            return fileName;
        }

        private string GenerateFileNameForKey(string setDirectoryFullPath, int attempt = 0)
        {
            var name = Guid.NewGuid() + ".dat";
            var fullFilePath = Path.Combine(setDirectoryFullPath, name);
            if (File.Exists(fullFilePath))
            {
                attempt++;
                if (attempt > 1000)
                {
                    throw new Exception("GenerateNewEntityFileName failed 1k times.");
                }
                return GenerateFileNameForKey(setDirectoryFullPath, attempt);
            }
            return name;
        }

        private string GenerateFolderNameForCollection(int attempt = 0)
        {
            var name = Guid.NewGuid().ToString();
            var directoryFullPath = Path.Combine(dataStoreDirectoryFullPath, name);
            if (Directory.Exists(directoryFullPath))
            {
                attempt++;
                if (attempt > 1000)
                {
                    throw new Exception("GenerateNewSetFolderName failed 1k times.");
                }
                return GenerateFolderNameForCollection(attempt);
            }
            return name;
        }

        private void Save()
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var jtw = new JsonTextWriter(sw))
                {
                    serializer.Serialize(jtw, collections);
                }
            }
            var contents = sb.ToString();

            ReliableFileOps.SaveFileContents(dataStoreDictionaryFileFullPath, contents);
        }
    }
}