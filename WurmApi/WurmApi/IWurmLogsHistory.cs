using System.Collections.Generic;
using System.Threading;
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

        /// <summary>
        /// Performs asynchronous logs history scan, returning all events matching parameters, sorted chronologically.
        /// If nothing has met search criteria, returns empty list.
        /// </summary>
        /// <param name="specificCharacterLogSearchParameters"></param>
        /// <param name="cancellationToken">a token, that can be used to cancel search job</param>
        /// <exception cref="InvalidSearchParametersException">Provided search parameters were invalid.</exception>
        /// <returns></returns>
        Task<IList<LogEntry>> Scan(LogSearchParameters specificCharacterLogSearchParameters, CancellationToken cancellationToken);
    }
}
