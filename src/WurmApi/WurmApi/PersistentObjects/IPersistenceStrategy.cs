namespace AldursLab.WurmApi.PersistentObjects
{
    interface IPersistenceStrategy
    {
        /// <summary>
        /// Returns Null, if data store does not contain object with this Id.
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="collectionId"></param>
        /// <returns></returns>
        /// <exception cref="PersistenceException">An error occured, while attempting to load data, from persistence store.</exception>
        string TryLoad(string objectId, string collectionId);

        /// <summary>
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="collectionId"></param>
        /// <param name="content"></param>
        /// <exception cref="PersistenceException">An error occured while, attempting to save data, to persistence store.</exception>
        void Save(string objectId, string collectionId, string content);
    }
}