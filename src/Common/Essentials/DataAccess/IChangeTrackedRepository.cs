namespace AldursLab.Essentials.DataAccess
{
    /// <summary>
    /// Repository of objects, implemented with ORM-side change tracking.
    /// </summary>
    public interface IChangeTrackedRepository
    {
    }

    /// <summary>
    /// Repository of objects of type <typeparam name="TEntity"></typeparam> with primary keys of type <typeparam name="TKey"></typeparam>,
    /// implemented with ORM-side change tracking.
    /// </summary>
    /// <typeparam name="TEntity">Type representing data entities.</typeparam>
    /// <typeparam name="TKey">Type representing primary key of data entities.</typeparam>
    public interface IChangeTrackedRepository<TEntity, in TKey> : IChangeTrackedRepository where TEntity : class
    {
        /// <summary>
        /// Marks the entity for addition to the store.
        /// </summary>
        /// <param name="entity"></param>
        void Add(TEntity entity);

        /// <summary>
        /// Gets an entity by its primary key. Entity is expected to exist in data store.
        /// </summary>
        /// <param name="id">Entity primary key value.</param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException">Entity with matching key was not found.</exception>
        TEntity GetById(TKey id);

        /// <summary>
        /// Searches for an entity by its primary key, returns the entity if found or null if not found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity TryGetById(TKey id);

        /// <summary>
        /// Marks the entity for deletion from the store.
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);
    }
}
