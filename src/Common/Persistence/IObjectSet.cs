namespace AldursLab.Persistence
{
    public interface IObjectSet
    {
        string Id { get; }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectId">Case insensitive</param>
        /// <returns></returns>
        T GetOrCreate<T>(string objectId) where T : class, new();

        /// <summary>
        /// </summary>
        /// <param name="objectId">Case insensitive</param>
        void Delete(string objectId);
    }

    public interface IObjectSet<T> where T : class, new()
    {
        string Id { get; }

        /// <summary>
        /// </summary>
        /// <param name="objectId">Case insensitive</param>
        /// <returns></returns>
        T GetOrCreate(string objectId);

        /// <summary>
        /// </summary>
        /// <param name="objectId">Case insensitive</param>
        void Delete(string objectId);
    }
}