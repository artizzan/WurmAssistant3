using System;
using System.Collections.Generic;
using System.Linq;

namespace AldurSoft.SimplePersist
{
    class PersistentSet<TEntity> : IPersistentSetInternal<TEntity>
        where TEntity : class, new()
    {
        #region Dependencies

        private IPersistentManagerInternal persistentManager;
        private EntityName entityName;

        public IPersistentManagerInternal PersistentManager
        {
            get { return persistentManager; }
            set
            {
                if (persistentManager != null)
                {
                    throw new InvalidOperationException("PersistentManager already set.");
                }
                persistentManager = value;
            }
        }

        public EntityName EntityName
        {
            get { return entityName; }
            set
            {
                if (entityName != null)
                {
                    throw new InvalidOperationException("PersistentManager already set.");
                }
                entityName = value;
            }
        }

        #endregion

        private readonly Dictionary<EntityKey, IPersistent<TEntity>> entities = new Dictionary<EntityKey, IPersistent<TEntity>>(); 

        public IPersistent<TEntity> Get(EntityKey key)
        {
            IPersistent<TEntity> entity;
            if (!entities.TryGetValue(key, out entity))
            {
                entity = new Persistent<TEntity>(key, this);
                entities.Add(key, entity);
            }
            return entity;
        }

        public IEnumerable<EntityKey> GetAllKeys()
        {
            return persistentManager.PersistenceStrategy.GetAllPersistedKeys(entityName).Concat(entities.Keys).Distinct();
        }

        public void DeleteAllPersisted()
        {
            var all = GetAllKeys().ToArray();
            foreach (var entityKey in all)
            {
                entities.Remove(entityKey);
                persistentManager.PersistenceStrategy.Delete(entityName, entityKey);
            }
        }

        public void SaveAllChanged()
        {
            foreach (var persistent in entities.Values)
            {
                persistent.SaveIfSetChanged();
            }
        }
    }
}