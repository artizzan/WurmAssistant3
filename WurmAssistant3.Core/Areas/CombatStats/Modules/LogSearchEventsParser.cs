using System;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Contracts;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Modules
{
    class LogSearchEventsParser : ICombatDataSource
    {
        readonly LogSearchParameters logSearchParameters;
        readonly IWurmApi wurmApi;

        readonly CombatResultsProcessor processor;

        bool processed;
        bool running;

        public LogSearchEventsParser(LogSearchParameters logSearchParameters, IWurmApi wurmApi)
        {
            if (logSearchParameters == null) throw new ArgumentNullException("logSearchParameters");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            this.logSearchParameters = logSearchParameters;
            this.wurmApi = wurmApi;

            CombatResults = new CombatResults();
            processor = new CombatResultsProcessor(CombatResults);
        }

        public event EventHandler<EventArgs> DataChanged;

        public CombatResults CombatResults { get; private set; }

        public async Task Process()
        {
            if (running) return;

            try
            {
                running = true;

                if (!processed)
                {
                    var logEntries = await wurmApi.LogsHistory.ScanAsync(logSearchParameters);
                    var filteredLogEvents =
                        logEntries.Where(
                            entry =>
                                entry.Timestamp >= logSearchParameters.MinDate
                                && entry.Timestamp <= logSearchParameters.MaxDate);

                    foreach (var entry in filteredLogEvents)
                    {
                        processor.ProcessEntry(entry);
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
