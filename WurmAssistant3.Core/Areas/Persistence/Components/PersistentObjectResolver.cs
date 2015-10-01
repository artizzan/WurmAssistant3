using System;
using System.Collections.Generic;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Core.Areas.Persistence.Contracts;
using JetBrains.Annotations;
using Ninject;
using Ninject.Parameters;

namespace AldursLab.WurmAssistant3.Core.Areas.Persistence.Components
{
    public class PersistentObjectResolver<T> : IPersistentObjectResolver<T> where T : class, IPersistentObject
    {
        readonly Dictionary<string, T> cache = new Dictionary<string, T>(); 
        readonly IKernel kernel;
        readonly PersistenceManager persistenceManager;

        public PersistentObjectResolver([NotNull] IKernel kernel, [NotNull] PersistenceManager persistenceManager)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");
            if (persistenceManager == null) throw new ArgumentNullException("persistenceManager");
            this.kernel = kernel;
            this.persistenceManager = persistenceManager;
        }

        /// <summary>
        /// Returns persistent object of given Id. If the object is not yet being tracked, it is created and loaded.
        /// </summary>
        /// <param name="persistentObjectId"></param>
        /// <returns></returns>
        public T Get(string persistentObjectId)
        {
            T obj;
            if (!cache.TryGetValue(persistentObjectId, out obj))
            {
                var persistentObjectIdParam = new ConstructorArgument("persistentObjectId", persistentObjectId);
                obj = kernel.Get<T>(persistentObjectIdParam);
                cache.Add(persistentObjectId, obj);
            }
            return obj;
        }

        public T GetDefault()
        {
            return Get("");
        }

        /// <summary>
        /// Stops tracking the object. 
        /// The object becomes eligible for GC and a future Get will start the tracking proces again.
        /// </summary>
        /// <param name="object"></param>
        public void Unload(T @object)
        {
            persistenceManager.StopTracking(@object);
            cache.Remove(@object.PersistentObjectId);
        }
    }
}