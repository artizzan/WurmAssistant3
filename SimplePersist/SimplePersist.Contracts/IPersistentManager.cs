namespace AldurSoft.SimplePersist
{
    /// <summary>
    /// Root manager for sets, that belong to single data store.
    /// </summary>
    public interface IPersistentManager
    {
        /// <summary>
        /// Gets a set of persistent objects for given entity id. 
        /// Sets provided by this manager are singletons per entity type and name.
        /// If set does not exist, it will be created.
        /// </summary>
        /// <exception cref="EntityTypeMismatchException">
        /// Entity set with this entity name has already been resolved using different TEntity type.
        /// </exception>
        IPersistentSet<TEntity> GetPersistentCollection<TEntity>(EntityName entitySetId) where TEntity : class, new();

        /// <summary>
        /// Saves all entities, that have been flagged as changed since last save.
        /// </summary>
        void SaveAllChanged();
    }
}