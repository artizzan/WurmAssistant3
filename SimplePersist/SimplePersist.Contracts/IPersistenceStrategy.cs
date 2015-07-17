using System.Collections.Generic;

namespace AldurSoft.SimplePersist
{
    /// <summary>
    /// Saves and loads entity serialized data from underlying data store.
    /// </summary>
    public interface IPersistenceStrategy
    {
        /// <summary>
        /// Saves the serialized data.
        /// </summary>
        void Save(EntityName entityName, EntityKey entityKey, string serializedData);

        /// <summary>
        /// Loads the serialized entity contents as a string.
        /// Returns empty string if nothing found.
        /// </summary>
        string Load(EntityName entityName, EntityKey entityKey);

        /// <summary>
        /// Checks if entity with this type and key exists in the data store.
        /// </summary>
        bool Exists(EntityName entityName, EntityKey entityKey);

        /// <summary>
        /// Returns keys for all entities persisted in the data store.
        /// </summary>
        /// <returns></returns>
        IEnumerable<EntityKey> GetAllPersistedKeys(EntityName entityName);

        /// <summary>
        /// Deletes persisted data from data store for this entity key.
        /// </summary>
        void Delete(EntityName entityName, EntityKey entityKey);

        /// <summary>
        /// Creates stamped backup of the entity contents.
        /// Returns full backup file path.
        /// </summary>
        string CreateCopy(EntityName entityName, EntityKey entityKey);
    }
}