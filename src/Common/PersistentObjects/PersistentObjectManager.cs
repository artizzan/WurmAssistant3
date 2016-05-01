using System;
using JetBrains.Annotations;

namespace AldursLab.PersistentObjects
{
    class PersistentObjectManager
    {
        readonly PersistenceManager persistenceManager;
        readonly WeakReference<IPersistentObject> persistentObject;

        bool disabled = false;

        public PersistentObjectManager([NotNull] ObjectCompositeKey compositeKey,
            [NotNull] IPersistentObject persistentObject, [NotNull] PersistenceManager persistenceManager)
        {
            if (compositeKey == null) throw new ArgumentNullException("compositeKey");
            if (persistentObject == null) throw new ArgumentNullException("persistentObject");
            if (persistenceManager == null) throw new ArgumentNullException("persistenceManager");
            this.CompositeKey = compositeKey;
            this.persistenceManager = persistenceManager;
            this.persistentObject = new WeakReference<IPersistentObject>(persistentObject);
        }

        public ObjectCompositeKey CompositeKey { get; private set; }

        public void Initialize()
        {
            IPersistentObject obj;
            if (!persistentObject.TryGetTarget(out obj))
            {
                // this should never happen if implemented correctly
                throw new InvalidOperationException(
                    string.Format("WeakReference died before BeginTracking, composite key: {0}", CompositeKey));
            }
            Load(obj);
            obj.PersistentStateSaveRequested += PersistentObjectOnPersistentStateSaveRequested;
            obj.PersistentDataLoaded();
        }

        public IPersistentObject TryGetObject()
        {
            IPersistentObject obj;
            persistentObject.TryGetTarget(out obj);
            if (obj == null)
            {
                // underlying object is GC'd
                persistenceManager.StopTracking(this);
            }
            return obj;
        }

        public void Save(bool forceSave = false)
        {
            if (disabled)
            {
                throw new InvalidOperationException("This object manager has been disabled, composite key: " + CompositeKey);
            }

            IPersistentObject obj;
            if (persistentObject.TryGetTarget(out obj))
            {
                if (obj.PersistibleChangesPending || forceSave)
                {
                    var data = persistenceManager.SerializationStrategy.Serialize(obj);
                    persistenceManager.PersistenceStrategy.Save(CompositeKey.CollectionId, CompositeKey.Key, data);
                    obj.PersistibleChangesPending = false;
                }
            }
            else
            {
                // underlying object is GC'd
                persistenceManager.StopTracking(this);
            }
        }

        void PersistentObjectOnPersistentStateSaveRequested(object sender, SaveEventArgs eventArgs)
        {
            Save(eventArgs.Force);
        }

        private void Load(IPersistentObject obj)
        {
            var data = persistenceManager.PersistenceStrategy.TryLoad(CompositeKey.CollectionId, CompositeKey.Key);
            if (!string.IsNullOrWhiteSpace(data))
            {
                persistenceManager.SerializationStrategy.PopulateFromSerialized(obj, data);
            }
        }

        public void Disable()
        {
            disabled = true;
        }

        public void DeleteData()
        {
            persistenceManager.PersistenceStrategy.TryDeleteData(CompositeKey.CollectionId, CompositeKey.Key);
        }
    }
}