using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AldursLab.WurmApi
{
    public interface IWurmCharacterLogs
    {
        /// <summary>
        /// Executes scan analogous to IWurmLogsHistory.
        /// </summary>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="logType"></param>
        /// <param name="scanResultOrdering"></param>
        /// <returns></returns>
        Task<IList<LogEntry>> ScanLogsAsync(DateTime minDate, DateTime maxDate, LogType logType,
            ScanResultOrdering scanResultOrdering = ScanResultOrdering.Ascending);

        /// <summary>
        /// Executes scan analogous to IWurmLogsHistory, further filtering all log entries to a specific server group.
        /// </summary>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="logType"></param>
        /// <param name="serverGroup"></param>
        /// <param name="scanResultOrdering"></param>
        /// <returns></returns>
        Task<IList<LogEntry>> ScanLogsServerGroupRestrictedAsync(DateTime minDate, DateTime maxDate, LogType logType,
            ServerGroup serverGroup, ScanResultOrdering scanResultOrdering = ScanResultOrdering.Ascending);
    }
}