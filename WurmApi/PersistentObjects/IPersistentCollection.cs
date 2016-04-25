namespace AldursLab.WurmApi.PersistentObjects
{
    interface IPersistentCollection
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectId"></param>
        /// <returns></returns>
        /// <exception cref="DeserializationErrorsException{TEntity}">
        /// At least one deserialization error occurred and was not handled by the Dto nor error handling strategy.
        /// </exception>
        IPersistent<T> GetObject<T>(string objectId) where T : Entity, new();
    }
}