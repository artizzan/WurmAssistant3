using System;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Modules
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
            processor = new CombatResultsProcessor(CombatStatus, logger);
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
                    // todo: get required logs from processor, run search for each log

                    var logTypes = processor.GetRequiredLogs();

                    foreach (var logType in logTypes)
                    {
                        var logEntries = await wurmApi.LogsHistory.ScanAsync(logSearchParameters);
                        var filteredLogEvents =
                            logEntries.Where(
                                entry =>
                                    entry.Timestamp >= logSearchParameters.MinDate
                                    && entry.Timestamp <= logSearchParameters.MaxDate);

                        foreach (var entry in filteredLogEvents)
                        {
                            await processor.ProcessEntryAsync(entry, LogType.Combat);
                        }
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
    }
}
