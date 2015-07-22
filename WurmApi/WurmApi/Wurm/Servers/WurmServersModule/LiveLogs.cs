using System.Collections.Generic;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Persistence.DataModel.WurmServersModel;
using AldurSoft.WurmApi.Wurm.CharacterServers;

namespace AldurSoft.WurmApi.Wurm.Servers.WurmServersModule
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

        private Task currentHandleNewLiveData = null;

        public async Task<TimeDetails> GetForServer(ServerName serverName)
        {
            if (currentHandleNewLiveData != null)
            {
                await currentHandleNewLiveData;
            }
            else
            {
                try
                {
                    var task = HandleNewLiveData(serverName);
                    currentHandleNewLiveData = task;
                    await task;
                }
                finally
                {
                    currentHandleNewLiveData = null;
                }
            }

            TimeDetails details;
            if (latestData.TryGetValue(serverName, out details))
            {
                return details;
            }

            return new TimeDetails();
        }

        private async Task HandleNewLiveData(ServerName serverName)
        {
            var newData = liveLogsDataQueue.Consume();
            foreach (var data in newData)
            {
                if (data.Uptime != null)
                {
                    var server = await wurmServerHistory.TryGetServer(data.Character, data.Uptime.Stamp.DateTime);
                    if (server != null)
                    {
                        TimeDetails details;
                        if (!latestData.TryGetValue(server, out details))
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
                    var server = await wurmServerHistory.TryGetServer(data.Character, data.WurmDateTime.Stamp.DateTime);
                    if (server != null)
                    {
                        TimeDetails details;
                        if (!latestData.TryGetValue(server, out details))
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