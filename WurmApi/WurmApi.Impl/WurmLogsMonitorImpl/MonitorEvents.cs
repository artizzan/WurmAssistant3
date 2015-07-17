using System.Collections.Generic;

namespace AldurSoft.WurmApi.Impl.WurmLogsMonitorImpl
{
    class MonitorEvents
    {
        private readonly List<LogEntry> logEntries = new List<LogEntry>();
        public LogFileInfo LogFileInfo { get; private set; }
        public IEnumerable<LogEntry> LogEntries { get { return logEntries; } }
        public string PmRecipient { get; private set; }

        public MonitorEvents(LogFileInfo logFileInfo)
        {
            LogFileInfo = logFileInfo;
            PmRecipient = logFileInfo.PmRecipientNormalized;
        }

        public void AddEvents(IEnumerable<LogEntry> entries)
        {
            this.logEntries.AddRange(entries);
        }
    }
}