using System;
using System.Collections.Generic;
using AldurSoft.WurmApi.Modules.Wurm.LogsMonitor;
using AldurSoft.WurmApi.Modules.Wurm.Servers.WurmServersModel;

namespace AldurSoft.WurmApi.Modules.Wurm.Servers
{
    class LiveLogsDataQueue : IDisposable
    {
        private readonly IWurmLogsMonitorInternal wurmLogsMonitor;

        // if parser becomes thread-unsafe, scope it to EventHandler
        private readonly LogEntriesParser logEntriesParser = new LogEntriesParser();

        readonly object locker = new object();

        public LiveLogsDataQueue(IWurmLogsMonitorInternal wurmLogsMonitor)
        {
            if (wurmLogsMonitor == null) throw new ArgumentNullException("wurmLogsMonitor");
            this.wurmLogsMonitor = wurmLogsMonitor;

            wurmLogsMonitor.SubscribeAllActiveInternal(EventHandler);
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
            lock (locker)
            {
                data.Add(new LiveLogsDataForCharacter(character, wurmDateTime, null));
            }
        }

        private void Add(CharacterName character, ServerUptimeStamped uptime)
        {
            lock (locker)
            {
                data.Add(new LiveLogsDataForCharacter(character, null, uptime));
            }
        }

        public IEnumerable<LiveLogsDataForCharacter> Consume()
        {
            lock (locker)
            {
                var result = data;
                data = new List<LiveLogsDataForCharacter>();
                return result;
            }
        }

        public void Dispose()
        {
            wurmLogsMonitor.UnsubscribeFromAll(EventHandler);
        }
    }
}
