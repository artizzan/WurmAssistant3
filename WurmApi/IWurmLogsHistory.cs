using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Provides means to search wurm log files.
    /// </summary>
    public interface IWurmLogsHistory
    {
        /// <summary>
        /// Performs asynchronous logs history scan, returning all events matching parameters, 
        /// sorted chronologically in reverse order (newest are first, oldest are last).
        /// If nothing has met search criteria, returns empty list.
        /// </summary>
        /// <param name="logSearchParameters"></param>
        /// <exception cref="InvalidSearchParametersException">Provided search parameters were invalid.</exception>
        /// <returns></returns>
        Task<IList<LogEntry>> ScanAsync(LogSearchParameters logSearchParameters);

        /// <summary>
        /// Performs asynchronous logs history scan, returning all events matching parameters, 
        /// sorted chronologically in reverse order (newest are first, oldest are last).
        /// If nothing has met search criteria, returns empty list.
        /// </summary>
        /// <param name="logSearchParameters"></param>
        /// <exception cref="InvalidSearchParametersException">Provided search parameters were invalid.</exception>
        /// <returns></returns>
        IList<LogEntry> Scan(LogSearchParameters logSearchParameters);

        /// <summary>
        /// Performs asynchronous logs history scan, returning all events matching parameters, 
        /// sorted chronologically in reverse order (newest are first, oldest are last).
        /// If nothing has met search criteria, returns empty list.
        /// </summary>
        /// <param name="logSearchParameters"></param>
        /// <param name="cancellationToken">a token, that can be used to cancel search job</param>
        /// <exception cref="InvalidSearchParametersException">Provided search parameters were invalid.</exception>
        /// <returns></returns>
        Task<IList<LogEntry>> ScanAsync(LogSearchParameters logSearchParameters, CancellationToken cancellationToken);

        /// <summary>
        /// Performs asynchronous logs history scan, returning all events matching parameters, 
        /// sorted chronologically in reverse order (newest are first, oldest are last).
        /// If nothing has met search criteria, returns empty list.
        /// </summary>
        /// <param name="logSearchParameters"></param>
        /// <param name="cancellationToken">a token, that can be used to cancel search job</param>
        /// <exception cref="InvalidSearchParametersException">Provided search parameters were invalid.</exception>
        /// <returns></returns>
        IList<LogEntry> Scan(LogSearchParameters logSearchParameters, CancellationToken cancellationToken);
    }
}
