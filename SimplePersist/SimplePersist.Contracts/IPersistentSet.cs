using System.Collections.Generic;

namespace AldurSoft.SimplePersist
{
    /// <summary>
    /// Collection of persistent objects. 
    /// </summary>
    public interface IPersistentSet
    {
        /// <summary>
        /// Logical name of the persistent collection.
        /// </summary>
        EntityName EntityName { get; }

        /// <summary>
        /// Saves all entities, that have been marked as changed since last save.
        /// </summary>
        void SaveAllChanged();

        /// <summary>
        /// Deletes all persisted data for this entity set. 
        /// (All data is permanently gone)
        /// </summary>
        void DeleteAllPersisted();
    }

    /// <summary>
    /// Collection of persistent objects of type TEntity, retrievable by key.
    /// Same persistent object instance is always returned for given key.
    /// </summary>
    public interface IPersistentSet<out TEntity> : IPersistentSet
        where TEntity : class, new()
    {
        /// <summary>
        /// Gets a persistent entity with provided key.
        /// If entity does not exist, creates new entity.
        /// </summary>
        IPersistent<TEntity> Get(EntityKey key);

        /// <summary>
        /// Gets keys for all entities cached in the set or persisted in the data store.
        /// </summary>
        IEnumerable<EntityKey> GetAllKeys();
    }
}