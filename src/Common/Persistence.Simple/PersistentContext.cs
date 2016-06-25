using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions;

namespace AldursLab.Persistence.Simple
{
    public class PersistentContext : IPersistentContext
    {
        readonly ISerializer serializer;
        readonly IDataStorage dataStorage;

        readonly Dictionary<string, ObjectSet> sets = new Dictionary<string, ObjectSet>();
        readonly object locker = new object();

        public PersistentContext(ISerializer serializer, IDataStorage dataStorage)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            if (dataStorage == null) throw new ArgumentNullException(nameof(dataStorage));
            this.serializer = serializer;
            this.dataStorage = dataStorage;
        }

        public void SaveChanged()
        {
            lock (locker)
            {
                foreach (var pc in sets.Values)
                {
                    pc.SaveChanged();
                }
            }
        }

        public void SaveAll()
        {
            lock (locker)
            {
                foreach (var pc in sets.Values)
                {
                    pc.SaveAll();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="setId">Case insensitive</param>
        /// <returns></returns>
        public IObjectSet GetOrCreateObjectSet(string setId)
        {
            return GetOrCreateObjectSetPrivate(setId);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId">Case insensitive</param>
        /// <returns></returns>
        public IObjectSet<T> GetOrCreateObjectSet<T>(string setId) where T : class, new()
        {
            var pc = GetOrCreateObjectSetPrivate(setId);
            return new StronglyTypedObjectSet<T>(pc);
        }

        /// <summary>
        /// </summary>
        /// <param name="setId">Case insensitive</param>
        public void DeleteObjectSet(string setId)
        {
            lock (locker)
            {
                ObjectSet pc;
                dataStorage.DeleteObjectSet(setId);
                if (sets.TryGetValue(setId.ToUpperInvariant(), out pc))
                {
                    sets.Remove(setId.ToUpperInvariant());
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="setId">Case insensitive</param>
        /// <returns></returns>
        ObjectSet GetOrCreateObjectSetPrivate(string setId)
        {
            PersistencePathValidator.ThrowIfPathInvalid(setId);

            lock (locker)
            {
                ObjectSet pc;
                if (!sets.TryGetValue(setId.ToUpperInvariant(), out pc))
                {
                    pc = new ObjectSet(setId, serializer, dataStorage, locker);
                    sets[setId.ToUpperInvariant()] = pc;
                }
                return pc;
            }
        }
    }
}
