namespace AldurSoft.SimplePersist
{
    public struct SerializationResult<TEntity>
        where TEntity : class, new()
    {
        private readonly TEntity deserializedEntity;
        private readonly SerializationErrors serializationErrors;

        public SerializationResult(TEntity deserializedEntity, SerializationErrors serializationErrors = null)
        {
            this.deserializedEntity = deserializedEntity;
            this.serializationErrors = serializationErrors;
        }

        /// <summary>
        /// Deserialized entity.
        /// </summary>
        public TEntity DeserializedEntity
        {
            get { return deserializedEntity; }
        }

        /// <summary>
        /// Contains all encountered errors, if any.
        /// </summary>
        public SerializationErrors SerializationErrors
        {
            get { return serializationErrors ?? new SerializationErrors(); }
        }

        public override string ToString()
        {
            return string.Format("Entity type: {0}", DeserializedEntity.GetType().FullName)
                   + (serializationErrors != null ? string.Format(", Errors: {0}", SerializationErrors) : string.Empty);
        }
    }
}