using System;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.CombatAssistant.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.CombatAssistant.Data.Combat;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatAssistant.Modules
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

            CombatStatus = new CombatStatus(characterName);
            processor = new CombatResultsProcessor(CombatStatus, logger, wurmApi);

            wurmApi.LogsMonitor.Subscribe(characterName, LogType.AllLogs, LogHandler);
        }

        public event EventHandler<EventArgs> DataChanged;

        public CombatStatus CombatStatus { get; private set; }

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
            wurmApi.LogsMonitor.Unsubscribe(characterName, LogHandler);
        }

        void LogHandler(object sender, LogsMonitorEventArgs logsMonitorEventArgs)
        {
            try
            {
                if (collecting)
                {
                    bool anyMatched = false;
                    foreach (var entry in logsMonitorEventArgs.WurmLogEntries)
                    {
                        try
                        {
                            anyMatched |= processor.ProcessEntry(entry, logsMonitorEventArgs.LogType);
                        }
                        catch (Exception exception)
                        {
                            logger.Error(exception, "Error at parsing live log event for combat stats, entry: " + entry);
                        }
                    }
                    if (anyMatched) OnDataChanged();
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "General error at parsing live log event for combat stats.");
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
