using System.Collections.Generic;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Provides means to search wurm log files.
    /// </summary>
    public interface IWurmLogsHistory
    {
        /// <summary>
        /// Performs asynchronous logs history scan, returning all events matching parameters, sorted chronologically.
        /// If nothing has met search criteria, returns empty list.
        /// </summary>
        /// <param name="specificCharacterLogSearchParameters"></param>
        /// <exception cref="InvalidSearchParametersException">Provided search parameters were invalid.</exception>
        /// <returns></returns>
        Task<IList<LogEntry>> Scan(LogSearchParameters specificCharacterLogSearchParameters);
    }
}
