using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.CombatAssistant.Data.Combat;
using AldursLab.WurmAssistant3.Areas.Logging;

namespace AldursLab.WurmAssistant3.Areas.CombatAssistant
{
    class LogSearchEventsParser : ICombatDataSource
    {
        readonly LogSearchParameters logSearchParameters;
        readonly IWurmApi wurmApi;

        readonly CombatResultsProcessor processor;

        bool processed;
        bool running;

        public LogSearchEventsParser(LogSearchParameters logSearchParameters, IWurmApi wurmApi, ILogger logger)
        {
            if (logSearchParameters == null) throw new ArgumentNullException("logSearchParameters");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            this.logSearchParameters = logSearchParameters;
            this.wurmApi = wurmApi;

            CombatStatus = new CombatStatus(logSearchParameters.CharacterName);
            processor = new CombatResultsProcessor(CombatStatus, logger, wurmApi);
        }

        public event EventHandler<EventArgs> DataChanged;

        public CombatStatus CombatStatus { get; private set; }

        public async Task Process()
        {
            if (running) return;

            try
            {
                running = true;

                if (!processed)
                {
                    // all events need to be aggregated first and sorted by timestamp
                    // so that processor can properly correlate events from different logs

                    var logTypes = processor.GetRequiredLogs();
                    var allEntries = new List<LogEntryWithLogType>();

                    foreach (var logType in logTypes)
                    {
                        logSearchParameters.LogType = logType;
                        var logEntries = await wurmApi.LogsHistory.ScanAsync(logSearchParameters);
                        var filteredLogEvents =
                            logEntries.Where(
                                entry =>
                                    entry.Timestamp >= logSearchParameters.MinDate
                                    && entry.Timestamp <= logSearchParameters.MaxDate);
                        allEntries.AddRange(filteredLogEvents.Select(entry => new LogEntryWithLogType(entry, logType)));
                    }

                    foreach (var entry in allEntries.OrderBy(type => type.LogEntry.Timestamp))
                    {
                        await processor.ProcessEntryAsync(entry.LogEntry, entry.LogType);
                    }


                    processed = true;
                    OnDataChanged();
                }
            }
            finally
            {
                running = false;
            }
        }

        protected void OnDataChanged()
        {
            var handler = DataChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        class LogEntryWithLogType
        {
            public LogEntry LogEntry { get; private set; }
            public LogType LogType { get; private set; }

            public LogEntryWithLogType(LogEntry logEntry, LogType logType)
            {
                if (logEntry == null) throw new ArgumentNullException("logEntry");
                LogEntry = logEntry;
                LogType = logType;
            }
        }
    }
}
