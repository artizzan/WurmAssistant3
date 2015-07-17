using System;
using System.Collections.Generic;

using AldurSoft.SimplePersist;
using AldurSoft.WurmApi.DataModel.WurmServersModel;

namespace AldurSoft.WurmApi.Impl.WurmServersImpl
{
    /// <summary>
    /// Manages information about wurm game servers.
    /// </summary>
    public class WurmServers : IWurmServers
    {
        private readonly IWurmLogsHistory wurmLogsHistory;
        private readonly IWurmLogsMonitor wurmLogsMonitor;
        private readonly IWurmServerList wurmServerList;
        private readonly IHttpWebRequests httpWebRequests;
        private readonly IWurmApiDataContext dataContext;
        private readonly IWurmCharacterDirectories wurmCharacterDirectories;
        private readonly IWurmServerHistory wurmServerHistory;
        private readonly ILogger logger;
        private readonly IThreadGuard threadGuard;
        private readonly Dictionary<ServerName, WurmServer> nameToServerMap = new Dictionary<ServerName, WurmServer>();

        private readonly WurmServerFactory wurmServerFactory;

        public WurmServers(
            IWurmLogsHistory wurmLogsHistory,
            IWurmLogsMonitor wurmLogsMonitor,
            IWurmServerList wurmServerList,
            IHttpWebRequests httpWebRequests,
            IWurmApiDataContext dataContext,
            IWurmCharacterDirectories wurmCharacterDirectories,
            IWurmServerHistory wurmServerHistory,
            ILogger logger,
            IThreadGuard threadGuard)
        {
            if (wurmLogsHistory == null) throw new ArgumentNullException("wurmLogsHistory");
            if (wurmLogsMonitor == null) throw new ArgumentNullException("wurmLogsMonitor");
            if (wurmServerList == null) throw new ArgumentNullException("wurmServerList");
            if (httpWebRequests == null) throw new ArgumentNullException("httpWebRequests");
            if (dataContext == null) throw new ArgumentNullException("dataContext");
            if (wurmCharacterDirectories == null) throw new ArgumentNullException("wurmCharacterDirectories");
            if (wurmServerHistory == null) throw new ArgumentNullException("wurmServerHistory");
            if (logger == null) throw new ArgumentNullException("logger");
            if (threadGuard == null) throw new ArgumentNullException("threadGuard");
            this.wurmLogsHistory = wurmLogsHistory;
            this.wurmLogsMonitor = wurmLogsMonitor;
            this.wurmServerList = wurmServerList;
            this.httpWebRequests = httpWebRequests;
            this.dataContext = dataContext;
            this.wurmCharacterDirectories = wurmCharacterDirectories;
            this.wurmServerHistory = wurmServerHistory;
            this.logger = logger;
            this.threadGuard = threadGuard;

            LiveLogsDataQueue liveLogsDataQueue = new LiveLogsDataQueue(wurmLogsMonitor, new LogEntriesParser());
            LiveLogs liveLogs = new LiveLogs(liveLogsDataQueue, wurmServerHistory);

            IPersistent<ServersData> persistent = dataContext.ServersData.Get(new EntityKey("WurmServers"));
            LogHistorySaved logHistorySaved = new LogHistorySaved(persistent);
            LogHistory logHistory = new LogHistory(wurmLogsHistory, wurmCharacterDirectories, wurmServerHistory, logHistorySaved, new LogEntriesParser());
            
            WebFeeds webFeeds = new WebFeeds(httpWebRequests, wurmServerList, logger);

            wurmServerFactory = new WurmServerFactory(
                liveLogs,
                logHistory,
                webFeeds,
                threadGuard);

            var descriptions = wurmServerList.All;
            foreach (var serverDescription in descriptions)
            {
                RegisterServer(serverDescription);
            }
        }

        private void RegisterServer(WurmServerInfo wurmServerInfo)
        {
            var normalizedName = wurmServerInfo.Name;
            if (this.nameToServerMap.ContainsKey(normalizedName))
            {
                throw new WurmApiException("Server already registered: " + wurmServerInfo);
            }

            var server = wurmServerFactory.Create(wurmServerInfo);

            this.nameToServerMap.Add(normalizedName, server);
        }

        public IEnumerable<IWurmServer> All
        {
            get
            {
                threadGuard.ValidateCurrentThread();
                return this.nameToServerMap.Values;
            }
        }

        public virtual IWurmServer TryGetByName(ServerName name)
        {
            if (name == null) throw new ArgumentNullException("name");
            threadGuard.ValidateCurrentThread();

            WurmServer server;
            this.nameToServerMap.TryGetValue(name, out server);
            return server;
        }
    }
}