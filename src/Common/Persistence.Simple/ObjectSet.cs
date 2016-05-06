using System;
using System.Collections.Generic;

namespace AldursLab.Persistence.Simple
{
    public class ObjectSet : IObjectSet
    {
        readonly string setId;
        readonly ISerializer serializer;
        readonly IDataStorage dataStorage;
        readonly Dictionary<string, PersistentObject> objects = new Dictionary<string, PersistentObject>();
        readonly object locker;

        public ObjectSet(string setId, ISerializer serializer, IDataStorage dataStorage, object locker)
        {
            if (setId == null) throw new ArgumentNullException(nameof(setId));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            if (dataStorage == null) throw new ArgumentNullException(nameof(dataStorage));
            if (locker == null) throw new ArgumentNullException(nameof(locker));
            this.setId = setId;
            this.serializer = serializer;
            this.dataStorage = dataStorage;
            this.locker = locker;
        }

        public string Id => setId;

        public void SaveChanged()
        {
            lock (locker)
            {
                foreach (var po in objects.Values)
                {
                    if (po.ChangedAndUnsaved)
                    {
                        po.SaveAndFlagUnchanged(serializer, dataStorage, setId);
                    }
                }
            }
        }

        public void SaveAll()
        {
            lock (locker)
            {
                foreach (var po in objects.Values)
                {
                    po.SaveAndFlagUnchanged(serializer, dataStorage, setId);
                }
            }
        }

        public T GetOrCreate<T>(string objectId) where T : class, new()
        {
            PersistencePathValidator.ThrowIfPathInvalid(objectId);

            lock (locker)
            {
                PersistentObject po;
                if (!objects.TryGetValue(objectId.ToUpperInvariant(), out po))
                {
                    var data = dataStorage.TryLoad(setId, objectId);
                    T o;
                    if (data != null)
                    {
                        o = serializer.Deserialize<T>(data);
                        po = new PersistentObject(objectId, o);
                    }
                    else
                    {
                        o = new T();
                        po = new PersistentObject(objectId, o);
                        po.SaveAndFlagUnchanged(serializer, dataStorage, setId);
                    }

                    objects[objectId.ToUpperInvariant()] = po;
                }
                return (T)po.Obj;
            }
        }

        public void Delete(string objectId)
        {
            lock (locker)
            {
                PersistentObject o;
                if (objects.TryGetValue(objectId.ToUpperInvariant(), out o))
                {
                    dataStorage.Delete(setId, o.Id);
                    objects.Remove(objectId.ToUpperInvariant());
                    o.StopTrackingChanges();
                }
            }
        }
    }
}