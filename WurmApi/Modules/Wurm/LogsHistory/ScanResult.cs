using System.Collections.Generic;

namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory
{
    class ScanResult
    {
        public IList<LogEntry> LogEntries { get; private set; }

        public ScanResult(IList<LogEntry> logEntries)
        {
            LogEntries = logEntries;
        }
    }
}