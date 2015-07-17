using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.SimplePersist
{
    /// <summary>
    /// Exposes a persistent entity and provides methods to manage its persistence.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IPersistent<out TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// Key for this entity
        /// </summary>
        EntityKey EntityKey { get; }

        /// <summary>
        /// Returns entity object, lazy loaded or created on first access if not available.
        /// </summary>
        TEntity Entity { get; }

        /// <summary>
        /// Returns true if SetChanged.
        /// </summary>
        bool Changed { get; }

        /// <summary>
        /// Saves the entity, if it's flagged as Changed. Sets Changed flag to false on successful save.
        /// </summary>
        void SaveIfSetChanged();

        /// <summary>
        /// Saves regardless of Changed flag, sets Changed flag to false.
        /// </summary>
        void Save();

        /// <summary>
        /// Marks the entity as changed.
        /// </summary>
        void SetChanged();

        /// <summary>
        /// Loads the Entity from the backing store
        /// </summary>
        void Load();

        /// <summary>
        /// Unloads the Entity without committing any changes. 
        /// (any unsaved changes are lost)
        /// </summary>
        void Unload();

        /// <summary>
        /// Checks if entity contents are loaded.
        /// </summary>
        /// <returns></returns>
        bool Loaded { get; }

        /// <summary>
        /// Checks if any data for this entity exists in data store.
        /// </summary>
        bool Persisted { get; }
    }
}
