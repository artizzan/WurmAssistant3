using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Persistence.Simple.Tests.Fixtures
{
    class InMemoryDataStore : IDataStorage
    {
        readonly Dictionary<string, Dictionary<string, string>> inMemoryStorage =
            new Dictionary<string, Dictionary<string, string>>();

        public void Save(string setId, string objectId, string data)
        {
            setId = setId.ToUpperInvariant();
            objectId = objectId.ToUpperInvariant();

            var set = GetOrCreateObjectSet(setId);
            set[objectId] = data;
        }

        public string TryLoad(string setId, string objectId)
        {
            setId = setId.ToUpperInvariant();
            objectId = objectId.ToUpperInvariant();

            var set = GetOrCreateObjectSet(setId);
            return TryGetDataFromSet(objectId, set);
        }

        public void Delete(string setId, string objectId)
        {
            setId = setId.ToUpperInvariant();
            objectId = objectId.ToUpperInvariant();

            var set = GetOrCreateObjectSet(setId.ToUpperInvariant());
            set.Remove(objectId.ToUpperInvariant());
        }

        public void DeleteObjectSet(string setId)
        {
            setId = setId.ToUpperInvariant();

            inMemoryStorage.Remove(setId.ToUpperInvariant());
        }

        public void DeleteAllObjectSets()
        {
            inMemoryStorage.Clear();
        }

        Dictionary<string, string> GetOrCreateObjectSet(string storagePath)
        {
            Dictionary<string, string> result;
            if (!inMemoryStorage.TryGetValue(storagePath, out result))
            {
                result = new Dictionary<string, string>();
                inMemoryStorage[storagePath] = result;
            }
            return result;
        }

        string TryGetDataFromSet(string objectId, Dictionary<string, string> set)
        {
            string result = null;
            set.TryGetValue(objectId, out result);
            return result;
        }
    }
}
