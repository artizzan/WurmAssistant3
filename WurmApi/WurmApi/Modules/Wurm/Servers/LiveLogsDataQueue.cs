using System;
using System.Collections.Generic;
using AldurSoft.WurmApi.Modules.DataContext.DataModel.WurmServersModel;

namespace AldurSoft.WurmApi.Modules.Wurm.Servers
{
    class LiveLogsDataQueue
    {
        private readonly IWurmLogsMonitor wurmLogsMonitor;
        private readonly LogEntriesParser logEntriesParser;

        public LiveLogsDataQueue(IWurmLogsMonitor wurmLogsMonitor, LogEntriesParser logEntriesParser)
        {
            if (wurmLogsMonitor == null) throw new ArgumentNullException("wurmLogsMonitor");
            if (logEntriesParser == null) throw new ArgumentNullException("logEntriesParser");
            this.wurmLogsMonitor = wurmLogsMonitor;
            this.logEntriesParser = logEntriesParser;

            wurmLogsMonitor.SubscribeAllActive(EventHandler);
        }

        private void EventHandler(object sender, LogsMonitorEventArgs logsMonitorEventArgs)
        {
            if (logsMonitorEventArgs.LogType == LogType.Event)
            {
                foreach (var wurmLogEntry in logsMonitorEventArgs.WurmLogEntries)
                {
                    var uptime = logEntriesParser.TryParseUptime(wurmLogEntry);
                    if (uptime != null)
                    {
                        Add(logsMonitorEventArgs.CharacterName, uptime);
                    }
                    else
                    {
                        var datetime = logEntriesParser.TryParseWurmDateTime(wurmLogEntry);
                        if (datetime != null)
                        {
                            Add(logsMonitorEventArgs.CharacterName, datetime);
                        }
                    }
                }
            }
        }

        private List<LiveLogsDataForCharacter> data = new List<LiveLogsDataForCharacter>();

        private void Add(CharacterName character, ServerDateStamped wurmDateTime)
        {
            data.Add(new LiveLogsDataForCharacter(character) { WurmDateTime = wurmDateTime });
        }

        private void Add(CharacterName character, ServerUptimeStamped uptime)
        {
            data.Add(new LiveLogsDataForCharacter(character) { Uptime = uptime });
        }

        public IEnumerable<LiveLogsDataForCharacter> Consume()
        {
            var result = data.ToArray();
            data = new List<LiveLogsDataForCharacter>();
            return result;
        }
    }
}
