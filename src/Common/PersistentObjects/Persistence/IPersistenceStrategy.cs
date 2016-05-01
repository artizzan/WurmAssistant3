namespace AldursLab.PersistentObjects.Persistence
{
    public interface IPersistenceStrategy
    {
        /// <summary>
        /// Returns null if data store does not contain data for this collectionId and key.
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        string TryLoad(string collectionId, string key);

        void Save(string collectionId, string key, string content);

        /// <summary>
        /// Attempts to clear all data matching given collection and key ids.
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="key"></param>
        void TryDeleteData(string collectionId, string key);
    }
}