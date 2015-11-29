using System;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data;
using AldursLab.WurmAssistant3.Core.Areas.CombatStats.Data.Combat;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Modules
{
    class LiveLogsEventsMonitor : IDisposable, ICombatDataSource
    {
        readonly string characterName;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;

        bool collecting = false;
        readonly CombatResultsProcessor processor;

        public LiveLogsEventsMonitor(string characterName, IWurmApi wurmApi, ILogger logger)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            this.characterName = characterName;
            this.wurmApi = wurmApi;
            this.logger = logger;

            CombatResults = new CombatResults();
            processor = new CombatResultsProcessor(CombatResults, characterName);

            wurmApi.LogsMonitor.Subscribe(characterName, LogType.Combat, CombatLogEventHandler);
        }

        public event EventHandler<EventArgs> DataChanged;

        public CombatResults CombatResults { get; private set; }

        public void Start()
        {
            collecting = true;
        }

        public void Pause()
        {
            collecting = false;
        }

        public void Dispose()
        {
            Pause();
            wurmApi.LogsMonitor.Unsubscribe(characterName, CombatLogEventHandler);
        }

        void CombatLogEventHandler(object sender, LogsMonitorEventArgs logsMonitorEventArgs)
        {
            try
            {
                if (collecting)
                {
                    foreach (var entry in logsMonitorEventArgs.WurmLogEntries)
                    {
                        processor.ProcessEntry(entry);
                    }
                    OnDataChanged();
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Error at parsing live log event for combat stats.");
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
