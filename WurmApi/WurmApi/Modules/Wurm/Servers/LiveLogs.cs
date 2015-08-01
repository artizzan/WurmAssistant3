using System.Collections.Generic;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Modules.DataContext.DataModel.WurmServersModel;

namespace AldurSoft.WurmApi.Modules.Wurm.Servers
{
    class LiveLogs
    {
        private readonly LiveLogsDataQueue liveLogsDataQueue;
        private readonly IWurmServerHistory wurmServerHistory;

        readonly Dictionary<ServerName, TimeDetails> latestData = new Dictionary<ServerName, TimeDetails>();

        public LiveLogs(LiveLogsDataQueue liveLogsDataQueue, IWurmServerHistory wurmServerHistory)
        {
            this.liveLogsDataQueue = liveLogsDataQueue;
            this.wurmServerHistory = wurmServerHistory;
        }

        public TimeDetails GetForServer(ServerName serverName)
        {
            HandleNewLiveData();

            TimeDetails details;
            if (latestData.TryGetValue(serverName, out details))
            {
                return details;
            }

            return new TimeDetails();
        }

        public void HandleNewLiveData()
        {
            var newData = liveLogsDataQueue.Consume();
            foreach (var data in newData)
            {
                if (data.Uptime != null)
                {
                    var serverName = wurmServerHistory.GetServer(data.Character, data.Uptime.Stamp.DateTime);
                    if (serverName != null)
                    {
                        TimeDetails details;
                        if (!latestData.TryGetValue(serverName, out details))
                        {
                            details = new TimeDetails();
                            latestData.Add(serverName, details);
                        }

                        if (details.ServerUptime.Stamp < data.Uptime.Stamp)
                        {
                            details.ServerUptime = new ServerUptimeStamped()
                            {
                                Uptime = data.Uptime.Uptime,
                                Stamp = data.Uptime.Stamp
                            };
                        }
                    }
                }
                if (data.WurmDateTime != null)
                {
                    var serverName = wurmServerHistory.GetServer(data.Character, data.WurmDateTime.Stamp.DateTime);
                    if (serverName != null)
                    {
                        TimeDetails details;
                        if (!latestData.TryGetValue(serverName, out details))
                        {
                            details = new TimeDetails();
                            latestData.Add(serverName, details);
                        }

                        if (details.ServerDate.Stamp < data.WurmDateTime.Stamp)
                        {
                            details.ServerDate = new ServerDateStamped()
                            {
                                WurmDateTime = data.WurmDateTime.WurmDateTime, 
                                Stamp = data.WurmDateTime.Stamp
                            };
                        }
                    }
                }
            }
        }
    }
}