using System;
using AldursLab.PersistentObjects;
using AldurSoft.WurmApi.Modules.Wurm.Servers.WurmServersModel;

namespace AldurSoft.WurmApi.Modules.Wurm.Servers
{
    class LogHistorySaved
    {
        private readonly IPersistent<ServersData> persistenceManager;

        public LogHistorySaved(IPersistent<ServersData> persistenceManager)
        {
            if (persistenceManager == null) throw new ArgumentNullException("persistenceManager");
            this.persistenceManager = persistenceManager;
        }

        public DateTimeOffset LastScanDate
        {
            get { return persistenceManager.Entity.LastScanDate; }
            set
            {
                persistenceManager.Entity.LastScanDate = value;
                persistenceManager.FlagAsChanged();
            }
        }

        public void UpdateHistoric(ServerName serverName, ServerDateStamped date)
        {
            ServerData data = GetServerData(serverName);
            if (data.LogHistory.ServerDate.Stamp < date.Stamp)
            {
                data.LogHistory.ServerDate = date;
                persistenceManager.FlagAsChanged();
            }
        }

        public void UpdateHistoric(ServerName serverName, ServerUptimeStamped uptime)
        {
            ServerData data = GetServerData(serverName);
            if (data.LogHistory.ServerUptime.Stamp < uptime.Stamp)
            {
                data.LogHistory.ServerUptime = uptime;
                persistenceManager.FlagAsChanged();
            }
        }

        public TimeDetails GetHistoricForServer(ServerName serverName)
        {
            ServerData data = GetServerData(serverName);
            return data.LogHistory;
        }

        private ServerData GetServerData(ServerName serverName)
        {
            ServerData data;
            if (!persistenceManager.Entity.ServerDatas.TryGetValue(serverName, out data))
            {
                data = new ServerData();
                persistenceManager.Entity.ServerDatas.Add(serverName, data);
                persistenceManager.FlagAsChanged();
            }
            return data;
        }
    }
}