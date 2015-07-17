using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AldurSoft.SimplePersist.Persistence.FlatFiles
{
    public class StorageDictionary
    {
        private readonly string dataStoreDirectoryFullPath;
        private const string DictionaryFileName = "storage.map";

        private readonly string dataStoreDictionaryFileFullPath;

        private readonly EntitySets entitySets;

        private readonly TransactionalFileOps fileOps = new TransactionalFileOps();

        readonly JsonSerializer serializer;

        public StorageDictionary(string dataStoreDirectoryFullPath)
        {
            this.dataStoreDirectoryFullPath = dataStoreDirectoryFullPath;
            dataStoreDictionaryFileFullPath = Path.Combine(dataStoreDirectoryFullPath, DictionaryFileName);

            serializer = new JsonSerializer { Formatting = Formatting.Indented };

            var dictionaryFileContents = fileOps.LoadFileContents(dataStoreDictionaryFileFullPath);
            if (!string.IsNullOrEmpty(dictionaryFileContents))
            {
                using (var sr = new StringReader(dictionaryFileContents))
                {
                    using (var jtr = new JsonTextReader(sr))
                    {
                        entitySets = serializer.Deserialize<EntitySets>(jtr);
                    }
                }
            }
            else
            {
                if (!Directory.Exists(dataStoreDirectoryFullPath))
                {
                    Directory.CreateDirectory(dataStoreDirectoryFullPath);
                }
                entitySets = new EntitySets();
            } 
        }

        public string GetDirectoryFullPathForSet(EntityName entityName)
        {
            Entities entities;
            if (!entitySets.EntitySetNameToEntities.TryGetValue(entityName.Value, out entities))
            {
                entities = AddEntitySetMapping(entityName);
            }
            return Path.Combine(dataStoreDirectoryFullPath, entities.DirectoryName);
        }

        public string GetFileFullPathForEntity(EntityName entityName, EntityKey entityKey)
        {
            Entities entities;
            if (!entitySets.EntitySetNameToEntities.TryGetValue(entityName.Value, out entities))
            {
                entities = AddEntitySetMapping(entityName);
            }

            string fileName;
            if (!entities.EntityKeyToFileName.TryGetValue(entityKey.Value, out fileName))
            {
                fileName = AddEntityKeyMapping(entityName, entityKey);
            }

            return Path.Combine(dataStoreDirectoryFullPath, entities.DirectoryName, fileName);
        }

        private Entities AddEntitySetMapping(EntityName entityName)
        {
            var entities = new Entities() { EntityName = entityName.Value, DirectoryName = GenerateNewSetFolderName(), };
            entitySets.EntitySetNameToEntities.Add(entityName.Value, entities);
            Save();
            return entities;
        }

        /// <summary>
        /// Returns file name
        /// </summary>
        private string AddEntityKeyMapping(EntityName entityName, EntityKey entityKey)
        {
            Entities entities;
            if (!entitySets.EntitySetNameToEntities.TryGetValue(entityName.Value, out entities))
            {
                entities = AddEntitySetMapping(entityName);
            }
            var fileName = GenerateNewEntityFileName(Path.Combine(dataStoreDirectoryFullPath, entities.DirectoryName));
            entities.EntityKeyToFileName.Add(
                entityKey.Value,
                fileName);
            Save();
            return fileName;
        }

        private string GenerateNewEntityFileName(string setDirectoryFullPath, int attempt = 0)
        {
            var name = Guid.NewGuid() + ".srl";
            var fullFilePath = Path.Combine(setDirectoryFullPath, name);
            if (File.Exists(fullFilePath))
            {
                attempt++;
                if (attempt > 1000)
                {
                    throw new SimplePersistException("GenerateNewEntityFileName failed 1k times.");
                }
                return GenerateNewEntityFileName(setDirectoryFullPath, attempt);
            }
            return name;
        }

        private string GenerateNewSetFolderName(int attempt = 0)
        {
            var name = Guid.NewGuid().ToString();
            var directoryFullPath = Path.Combine(dataStoreDirectoryFullPath, name);
            if (Directory.Exists(directoryFullPath))
            {
                attempt++;
                if (attempt > 1000)
                {
                    throw new SimplePersistException("GenerateNewSetFolderName failed 1k times.");
                }
                return GenerateNewSetFolderName(attempt);
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
                    serializer.Serialize(jtw, entitySets);
                }
            }
            var contents = sb.ToString();

            fileOps.SaveFileContents(dataStoreDictionaryFileFullPath, contents);
        }

        public IEnumerable<EntityKey> GetAllKeys(EntityName entityName)
        {
            Entities entities;
            if (entitySets.EntitySetNameToEntities.TryGetValue(entityName.Value, out entities))
            {
                return entities.EntityKeyToFileName.Keys.Select(s => new EntityKey(s));
            }

            return new EntityKey[0];
        }

        public void DeleteMapping(EntityName entityName, EntityKey entityKey)
        {
            Entities entities;
            if (entitySets.EntitySetNameToEntities.TryGetValue(entityName.Value, out entities))
            {
                entities.EntityKeyToFileName.Remove(entityKey.Value);
            }
        }
    }
}
