using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.PersistentObjects.Persistence;
using AldursLab.PersistentObjects.Serialization;
using JetBrains.Annotations;

namespace AldursLab.PersistentObjects
{
    public class PersistenceManager
    {
        readonly PersistenceManagerConfig config;

        readonly Dictionary<ObjectCompositeKey, PersistentObjectManager> objects =
            new Dictionary<ObjectCompositeKey, PersistentObjectManager>();

        public PersistenceManager([NotNull] PersistenceManagerConfig config,
            ISerializationStrategy serializationStrategy, IPersistenceStrategy persistenceStrategy)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            this.config = config;

            SerializationStrategy = serializationStrategy;
            PersistenceStrategy = persistenceStrategy;
        }

        public ISerializationStrategy SerializationStrategy { get; private set; }

        public IPersistenceStrategy PersistenceStrategy { get; private set; }

        /// <summary>
        /// Populates the object with previously serialized state, begins tracking it for changes and returns it.
        /// If the object is already tracked, an exception is thrown, unless returnExistingInsteadOfException is enabled.
        /// If ReturnExistingInsteadOfException is enabled, as objects are identified by collection and id,
        /// returned object is not guaranteed to be of type of the object parameter.
        /// </summary>
        /// <param name="object"></param>
        /// <param name="returnExistingInsteadOfException"></param>
        /// <returns></returns>
        public object LoadAndStartTracking(object @object, bool returnExistingInsteadOfException = false)
        {
            IPersistentObject inface = CastToInterface(@object);
            return LoadAndStartTracking(inface, returnExistingInsteadOfException);
        }

        /// <summary>
        /// Populates the object with previously serialized state, begins tracking it for changes and returns it.
        /// If the object is already tracked, an exception is thrown, unless returnExistingInsteadOfException is enabled.
        /// ReturnExistingInsteadOfException may still fail, if object cannot be cast to desired type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="returnExistingInsteadOfException"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">
        /// object with same collectionId and persistentObjectId is already tracked, but is of incompatible type to the requested one
        /// </exception>
        public T LoadAndStartTracking<T>([NotNull] T @object, bool returnExistingInsteadOfException = false)
            where T : class, IPersistentObject
        {
            var result = Preprocess(@object);
            ObjectCompositeKey compositeKey = result.ObjectCompositeKey;
            IPersistentObject persistentObject = result.PersistentObject;

            PersistentObjectManager manager;
            if (objects.TryGetValue(compositeKey, out manager))
            {
                if (returnExistingInsteadOfException)
                {
                    var obj =  manager.TryGetObject();
                    if (obj == null)
                    {
                        // object is referenced weakly, it may have been GC'd recently
                        // if so, continue and load it again
                    }
                    else
                    {
                        return (T) obj;
                    }
                }
                else
                {
                    throw new PersistentObjectAlreadyTrackedException(
                        string.Format("object with the same composite key already exists. Composite key: [ {0} ]",
                            compositeKey.ToString()));
                }
            }

            var objectManager = new PersistentObjectManager(compositeKey, persistentObject, this);
            objectManager.Initialize();

            objects[compositeKey] = objectManager;

            return @object;
        }

        public void StopTracking(object @object, bool deleteData = false)
        {
            IPersistentObject inface = CastToInterface(@object);
            StopTracking(inface, deleteData);
        }

        public void StopTracking<T>([NotNull] T @object, bool deleteData = false) where T : class, IPersistentObject
        {
            var result = Preprocess(@object);
            PersistentObjectManager manager;
            if (objects.TryGetValue(result.ObjectCompositeKey, out manager))
            {
                manager.Disable();
                if (deleteData) manager.DeleteData();
                objects.Remove(manager.CompositeKey);
            }
        }

        PreprocessResult Preprocess<T>([NotNull] T @object) where T : class, IPersistentObject
        {
            if (@object == null)
                throw new ArgumentNullException("object");

            var attribute = @object.GetType().GetAttributes<PersistentObjectAttribute>(true).FirstOrDefault();

            if (attribute == null)
                throw new ArgumentException("object must be decorated with " + typeof(PersistentObjectAttribute).Name);

            var collectionId = attribute.CollectionId;
            var inface = CastToInterface(@object);

            var key = inface.PersistentObjectId;
            if (key == null)
            {
                throw new ArgumentException("object key cannot be null.");
            }
            if (collectionId.Length > 1000)
            {
                throw new ArgumentException("object key must be less than 1000 characters. Lets be sane here.");
            }

            return new PreprocessResult()
            {
                ObjectCompositeKey = new ObjectCompositeKey(collectionId, key),
                PersistentObject = inface
            };
        }

        IPersistentObject CastToInterface(object persistentObject)
        {
            var inface = persistentObject as IPersistentObject;

            if (inface == null)
                throw new ArgumentException("object must implement " + typeof(IPersistentObject).Name);

            return inface;
        }

        public void SavePending()
        {
            Save(false);
        }

        public void SaveAll()
        {
            Save(true);
        }

        void Save(bool forceSave)
        {
            foreach (var persistentObjectManager in objects.Values.ToArray())
            {
                persistentObjectManager.Save(forceSave);
            }
        }

        internal void StopTracking(PersistentObjectManager manager)
        {
            objects.Remove(manager.CompositeKey);
        }

        class PreprocessResult
        {
            public ObjectCompositeKey ObjectCompositeKey { get; set; }
            public IPersistentObject PersistentObject { get; set; }
        }
    }
}
