using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.SimplePersist
{
    internal class Persistent<TEntity> : IPersistent<TEntity>
        where TEntity : class, new()
    {
        private readonly IPersistentSetInternal<TEntity> persistentSet;
        private TEntity entity;

        public Persistent(
            EntityKey entityKey,
            IPersistentSetInternal<TEntity> persistentSet)
        {
            this.EntityKey = entityKey;
            this.persistentSet = persistentSet;
        }

        public EntityKey EntityKey { get; private set; }

        public TEntity Entity
        {
            get { return entity ?? (LoadEntity()); }
        }

        private TEntity LoadEntity()
        {
            var serialized = persistentSet.PersistentManager.PersistenceStrategy.Load(
                persistentSet.EntityName,
                EntityKey);

            var deserializeResult =
                persistentSet.PersistentManager.SerializationStrategy.Deserialize<TEntity>(serialized);
            if (deserializeResult.SerializationErrors.Exceptions.Any())
            {
                var backupFullFilePath =
                    persistentSet.PersistentManager.PersistenceStrategy.CreateCopy(persistentSet.EntityName, EntityKey);
                persistentSet.PersistentManager.Logger.Log(
                    string.Join(
                        ", ",
                        string.Format(
                            "Deserialization error at load of entity {0}, data backed up to file: {1}",
                            this.ToString(),
                            backupFullFilePath),
                        deserializeResult.ToString()),
                    Severity.Warning);
            }
            entity = deserializeResult.DeserializedEntity;

            return entity;
        }

        public bool Changed { get; private set; }

        public void SaveIfSetChanged()
        {
            if (Changed)
            {
                SaveInner();
            }
        }

        public void Save()
        {
            SaveInner();
        }

        private void SaveInner()
        {
            var serialized = persistentSet.PersistentManager.SerializationStrategy.Serialize(Entity);
            persistentSet.PersistentManager.PersistenceStrategy.Save(persistentSet.EntityName, EntityKey, serialized);
            Changed = false;
        }

        public void SetChanged()
        {
            Changed = true;
        }

        public void Load()
        {
            LoadEntity();
        }

        public void Unload()
        {
            Changed = false;
            entity = null;
        }

        public bool Loaded { get { return entity != null; } }

        public bool Persisted
        {
            get { return persistentSet.PersistentManager.PersistenceStrategy.Exists(persistentSet.EntityName, EntityKey); }
        }

        public override string ToString()
        {
            return string.Format(
                "EntityKey: {0}, EntitySet: {1}, EntityType: {2}",
                EntityKey,
                persistentSet.EntityName,
                typeof(TEntity).FullName);
        }
    }
}
