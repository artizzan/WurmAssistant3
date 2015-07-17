using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.SimplePersist.Persistence.FlatFiles
{
    public class FlatFilesPersistenceStrategy : IPersistenceStrategy
    {
        private readonly StorageDictionary storageDictionary;
        private readonly TransactionalFileOps fileOps = new TransactionalFileOps();

        public FlatFilesPersistenceStrategy(string dataStoreDirectoryFullPath)
        {
            if (dataStoreDirectoryFullPath == null) throw new ArgumentNullException("dataStoreDirectoryFullPath");

            storageDictionary = new StorageDictionary(dataStoreDirectoryFullPath);
        }

        public void Save(EntityName entityName, EntityKey entityKey, string serializedData)
        {
            var filepath = storageDictionary.GetFileFullPathForEntity(entityName, entityKey);
            fileOps.SaveFileContents(filepath, serializedData);
        }

        public string Load(EntityName entityName, EntityKey entityKey)
        {
            var filepath = storageDictionary.GetFileFullPathForEntity(entityName, entityKey);
            var data = fileOps.LoadFileContents(filepath);
            return data;
        }

        public bool Exists(EntityName entityName, EntityKey entityKey)
        {
            var filepath = storageDictionary.GetFileFullPathForEntity(entityName, entityKey);
            return File.Exists(filepath);
        }

        public IEnumerable<EntityKey> GetAllPersistedKeys(EntityName entityName)
        {
            return storageDictionary.GetAllKeys(entityName);
        }

        public void Delete(EntityName entityName, EntityKey entityKey)
        {
            var filepath = storageDictionary.GetFileFullPathForEntity(entityName, entityKey);
            File.Delete(filepath);
            storageDictionary.DeleteMapping(entityName, entityKey);
        }

        public string CreateCopy(EntityName entityName, EntityKey entityKey)
        {
            var filepath = storageDictionary.GetFileFullPathForEntity(entityName, entityKey);
            var backupFilePath = filepath + ".local-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffffff") + ".bak";
            File.Copy(filepath, backupFilePath);
            return backupFilePath;
        }
    }
}
