using System.Collections.Generic;

namespace AldursLab.WurmApi.Modules.Wurm.LogsMonitor
{
    class MonitorEvents
    {
        private readonly List<LogEntry> logEntries = new List<LogEntry>();
        public LogFileInfo LogFileInfo { get; private set; }
        public IEnumerable<LogEntry> LogEntries => logEntries;
        public string PmRecipientNormalized { get; private set; }

        public MonitorEvents(LogFileInfo logFileInfo)
        {
            LogFileInfo = logFileInfo;
            PmRecipientNormalized = logFileInfo.PmRecipientNormalized;
        }

        public void AddEvents(IEnumerable<LogEntry> entries)
        {
            logEntries.AddRange(entries);
        }
    }
}