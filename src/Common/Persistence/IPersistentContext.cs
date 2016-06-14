namespace AldursLab.Persistence
{
    public interface IPersistentContext
    {
        void SaveChanged();
        void SaveAll();

        /// <summary>
        /// </summary>
        /// <param name="setId">Case insensitive</param>
        /// <returns></returns>
        IObjectSet GetOrCreateObjectSet(string setId);

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId">Case insensitive</param>
        /// <returns></returns>
        IObjectSet<T> GetOrCreateObjectSet<T>(string setId) where T : class, new();

        /// <summary>
        /// </summary>
        /// <param name="setId">Case insensitive</param>
        void DeleteObjectSet(string setId);
    }
}