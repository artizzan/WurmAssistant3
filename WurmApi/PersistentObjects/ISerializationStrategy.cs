namespace AldursLab.WurmApi.PersistentObjects
{
    interface ISerializationStrategy
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="DeserializationErrorsException{TEntity}">
        /// At least one deserialization error occurred and was not handled. Decision is required. 
        /// A fallback TEntity should be available, with as much data, as was successfully deserialized.
        /// </exception>
        TEntity Deserialize<TEntity>(string source) where TEntity : Entity, new();

        string Serialize<TEntity>(TEntity entity) where TEntity : Entity, new();
    }
}