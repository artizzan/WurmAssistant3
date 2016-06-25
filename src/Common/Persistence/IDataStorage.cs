using System.Collections.Generic;

namespace AldursLab.Persistence
{
    /// <summary>
    /// Paths and Id's should be case insensitive (for english alphabet subset only).
    /// </summary>
    public interface IDataStorage
    {
        /// <param name="setId">Case insensitive</param>
        /// <param name="objectId">Case insensitive</param>
        /// <param name="data"></param>
        void Save(string setId, string objectId, string data);
        /// <param name="setId">Case insensitive</param>
        /// <param name="objectId">Case insensitive</param>
        string TryLoad(string setId, string objectId);

        /// <param name="setId">Case insensitive</param>
        /// <param name="objectId">Case insensitive</param>
        void Delete(string setId, string objectId);

        /// <param name="setId">Case insensitive</param>
        void DeleteObjectSet(string setId);
        void DeleteAllObjectSets();
    }
}