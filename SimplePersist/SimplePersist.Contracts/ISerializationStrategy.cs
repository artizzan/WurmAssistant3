namespace AldurSoft.SimplePersist
{
    /// <summary>
    /// Serializes and deserializes entities.
    /// </summary>
    public interface ISerializationStrategy
    {
        /// <summary>
        /// Deserializes string into an entity of given type.
        /// Gracefully handles all errors, see remarks for details.
        /// All encountered errors are appended to the result.
        /// </summary>
        /// <remarks>
        /// New members should have default values as expected by class definition.
        /// Missing members should be ignored, previous value does not have to remain in data store.
        /// If changed member is encountered and can't be deserialized, SerializationErrors needs to be set in the result.
        /// </remarks>
        SerializationResult<TEntity> Deserialize<TEntity>(string source) where TEntity : class, new();

        /// <summary>
        /// Serializes the entity into a string.
        /// </summary>
        string Serialize<TEntity>(TEntity entity) where TEntity : class, new();
    }
}